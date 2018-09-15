const fs = require("fs");
const path = require("path");
const xml = require("xml2js");
const csv = require("csv-parse/lib/sync");

const appdata = process.env.APPDATA;
const filenameBase = "corsair_cue";

const getSensorLogFolder = () => {
    return new Promise((resolve, reject) => {
        fs.readFile(path.join(appdata, "Corsair/CUE/config.cuecfg"), 'utf8', (err, data) => {
            if (err) return reject(err);
            xml.parseString(data, (err, result) => {
                if (err) return reject("Couldn't locate iCUE configuration.");
                const sensorLogging = result.config.map.filter(map => map.$.name === "SensorLogging")[0];
                if (sensorLogging) resolve(sensorLogging.value.filter(value => value.$.name === "Folder")[0]._);
                else reject("Sensor logging not configured.");
            })
        });
    });
};

const getLatestLogFile = dir => {
    return new Promise((resolve, reject) => {
        fs.readdir(dir, (err, files) => {
            if (err) return reject("Sensor log directory not found or couldn't be read.");
            files = files.filter(file => file.startsWith(filenameBase)).sort();
            if (files.length === 0) return reject("No sensor logs found.");
            resolve(path.join(dir, files[files.length - 1]));
        })
    });
};

const getLatestSensorValues = file => {
    return new Promise((resolve, reject) => {
        fs.readFile(file, 'utf8', (err, data) => {
            if (err) return reject("Couldn't read sensor log file.");
            const values = csv(data, { columns: true });
            resolve({
                source: "iCUE",
                lastUpdate: values[values.length - 1]["Timestamp"],
                sensors: Object.entries(values[values.length - 1])
                    .filter(([key, value]) => key !== "Timestamp")
                    .map(([key, value]) => ({ name: key, value: parseFloat(value) }))
            });
        })
    });
};

const getSensors = () =>
    getSensorLogFolder()
    .then(getLatestLogFile)
    .then(getLatestSensorValues)
    .then(results => ({
        source: "iCUE",
        sensors: results
    }));

const deleteSensorLogs = () =>
    getSensorLogFolder()
    .then(dir => {
        fs.readdir(dir, (err, files) => {
            if (err) return reject(err);
            for (const file of files) {
                fs.unlink(path.join(dir, file), err => {
                if (err) throw err;
                });
            }
        })
    })

module.exports = {
    getSensors: getSensors,
    deleteSensorLogs: deleteSensorLogs
};