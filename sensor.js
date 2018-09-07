const edge = require("edge-js");
const fs = require("fs");
const path = require("path");

const dll = "lib/Sensors.SoftwareInterface.dll";

const getSpeedFanData = function getSpeedFanData() {
    // Import library function
    const getData = edge.func({
        assemblyFile: path.join(__dirname, dll),
        typeName: "Sensors.SoftwareInterface.SpeedFan.SpeedFanInterface"
    });

    return new Promise((resolve, reject) => {
        // Get data from SpeedFan
        getData("", function (error, result) {
            if (error || !result) return reject(error);

            var out = result;

            // Divide temps by 100
            for (var i = 0; i < out.temps.length; i++) {
                out.temps[i].value /= 100;
            }

            // Divide volts by 100
            for (var i = 0; i < out.volts.length; i++) {
                out.volts[i].value /= 100;
            }

            resolve(out);
        });
    });
}

const getAISuite2Data = function () {
    // Import library function
    const getData = edge.func({
        assemblyFile: path.join(__dirname, dll),
        typeName: "Sensors.SoftwareInterface.AISuite.AISuite2Interface"
    });

    return new Promise((resolve, reject) => {
        // Get data from AI Suite
        getData("", function (error, result) {
            if (error || !result) return reject(error);

            var out = result;

            // Divide temps by 10
            for (var i = 0; i < out.temps.length; i++) {
                out.temps[i].value /= 10;
            }

            // Divide volts by 1000
            for (var i = 0; i < out.volts.length; i++) {
                out.volts[i].value /= 1000;
            }

            resolve(out);
        });
    });
}

module.exports = {
    getSpeedFanData: getSpeedFanData,
    getAISuite2Data: getAISuite2Data
};