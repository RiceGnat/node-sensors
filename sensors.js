const edge = require("edge-js");
const path = require("path");
const icue = require("icue-sensor-logs");

const dll = "lib/Sensors.SoftwareInterface.dll";

const formatError = error => {
    var out = {
        message: error.message
    };

    if (error.InnerException)
        out.exception = {
            name: error.InnerException.name,
            message: error.InnerException.message
        };

    return out;
};

const getSpeedFanData = () => {
    // Import library function
    const getData = edge.func({
        assemblyFile: path.join(__dirname, dll),
        typeName: "Sensors.SoftwareInterface.SpeedFan.SpeedFanInterface"
    });

    return new Promise((resolve, reject) => {
        // Get data from SpeedFan
        getData("", function (error, result) {
            if (error) return reject(formatError(error));

            var i, out = result;

            // Divide temps by 100
            for (i = 0; i < out.temps.length; i++) {
                out.temps[i].value /= 100;
            }

            // Divide volts by 100
            for (i = 0; i < out.volts.length; i++) {
                out.volts[i].value /= 100;
            }

            resolve(out);
        });
    });
};

const getAISuite2Data = () => {
    // Import library function
    const getData = edge.func({
        assemblyFile: path.join(__dirname, dll),
        typeName: "Sensors.SoftwareInterface.AISuite.AISuite2Interface"
    });

    return new Promise((resolve, reject) => {
        // Get data from AI Suite
        getData("", function (error, result) {
            if (error) return reject(formatError(error));

            var i, out = result;

            // Divide temps by 10
            for (i = 0; i < out.temps.length; i++) {
                out.temps[i].value /= 10;
            }

            // Divide volts by 1000
            for (i = 0; i < out.volts.length; i++) {
                out.volts[i].value /= 1000;
            }

            resolve(out);
        });
    });
};

const getiCUEData = () => {
    return icue.getSensors();
}

module.exports = {
    getSpeedFanData: getSpeedFanData,
    getAISuite2Data: getAISuite2Data,
    getiCUEData: getiCUEData
};