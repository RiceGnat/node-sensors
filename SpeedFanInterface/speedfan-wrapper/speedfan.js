const edge = require("edge");
const fs = require("fs");
const path = require("path");

const config = require("./config.json");

const getSensorData = edge.func({
    assemblyFile: config.dllPath ? config.dllPath : path.join(__dirname, "SpeedFanInterface.dll"),
    typeName: "SpeedFan.SpeedFanInterface"
});

const poll = function (getAll, callback) {
    // Load SpeedFan sensor config file
    fs.readFile(path.join(config.speedfanLocation, "speedfansens.cfg"), "utf8", function (err, data) {
        if (err) {
            callback(err);
        }

        // Read sensor names from config file
        var tempNames = [];
        var tempActive = [];
        var fanNames = [];
        var fanActive = [];
        var voltNames = [];
        var voltActive = [];
        var wild = "(?:(?!xxx end)[\\s\\S])*";
        var pattern = `xxx Temp${wild}name=(.*)${wild}active=(.*)${wild}xxx end`;
        var regex, match;

        regex = new RegExp(pattern, "g");
        match = regex.exec(data);
        while (match != null) {
            tempNames.push(match[1]);
            tempActive.push(match[2] == "true");
            match = regex.exec(data);
        }

        regex = new RegExp(pattern.replace("Temp", "Fan"), "g");
        match = regex.exec(data);
        while (match != null) {
            fanNames.push(match[1]);
            fanActive.push(match[2] == "true");
            match = regex.exec(data);
        }

        regex = new RegExp(pattern.replace("Temp", "Volt"), "g");
        match = regex.exec(data);
        while (match != null) {
            voltNames.push(match[1]);
            voltActive.push(match[2] == "true");
            match = regex.exec(data);
        }

        // Function to pair names with values (assumes same order)
        const pairNamesValues = function (names, values, active, divideBy) {
            var pairs = [];
            for (var i = 0; i < names.length; i++) {
                if (active[i] || getAll) {
                    pairs.push({
                        name: names[i],
                        value: values[i] / (divideBy ? divideBy : 1)
                    });
                }
            }
            return pairs;
        }

        // Get data from SpeedFan
        getSensorData("", function (error, result) {
            if (error || !result) {
                return callback(error, null);
            }

            var out = {};

            out.temps = pairNamesValues(tempNames, result.temps, tempActive, 100);
            out.fans = pairNamesValues(fanNames, result.fans, fanActive);
            out.volts = pairNamesValues(voltNames, result.volts, voltActive, 100);

            if (typeof callback === "function") callback(null, out);
        });
    });
}

module.exports = {
    poll: poll
}