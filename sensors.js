const edge = require("edge-js");
const path = require("path");
const icue = require("./icue/sensor-logs");

const dll = "lib/Sensors.SoftwareInterface.dll";

const formatError = error => {
    var out = {
        message: error.message || error
    };

    if (error.InnerException)
        out.exception = {
            name: error.InnerException.name,
            message: error.InnerException.message
        };

    return out;
};

const getDataFrom = typeName => {
    // Import library function
    const getData = edge.func({
        assemblyFile: path.join(__dirname, dll),
        typeName: typeName
    });

    return new Promise((resolve, reject) => {
        getData("", function (error, result) {
            if (error) return reject(formatError(error));
            resolve(result);
        });
    });
};

const getSpeedFanData = () => getDataFrom("Sensors.SoftwareInterface.SpeedFan.SpeedFanInterface");
const getAISuite2Data = () => getDataFrom("Sensors.SoftwareInterface.AISuite.AISuite2Interface");
const getOpenHWMonitorData = () => getDataFrom("Sensors.SoftwareInterface.OpenHardwareMonitor.OpenHardwareMonitorInterface");

const getiCUEData = () => icue.getSensors().catch(error => { throw formatError(error); });

module.exports = {
    getSpeedFanData: getSpeedFanData,
    getAISuite2Data: getAISuite2Data,
    getOpenHWMonitorData: getOpenHWMonitorData,
    getiCUEData: getiCUEData
};