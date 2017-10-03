const edge = require("edge");
const fs = require("fs");
const path = require("path");

const config = require("./config.json");

const getSensorData = edge.func({
    assemblyFile: path.join(__dirname, "SpeedFanInterface.dll"),
    typeName: "SpeedFan.SpeedFanInterface"
});

const poll = function (callback) {
    fs.readFile(path.join(config.speedfanLocation, "speedfansens.cfg"), "utf8", function (err, data) {
        if (err) {
            callback(err);
        }

        var tempNames = [];
        var tempActive = [];

        var pattern = /xxx Temp(?:(?!xxx end)[\s\S])*name=(.*)(?:(?!xxx end)[\s\S])*active=(.*)(?:(?!xxx end)[\s\S])*xxx end/g;
        var match = pattern.exec(data);

        while (match != null) {
            tempNames.push(match[1]);
            tempActive.push(match[2] == "true");
            match = pattern.exec(data);
        }

        getSensorData("", function (error, result) {
            var out = {};
            out.temps = matchTempNamesValues(tempNames, result.temps, tempActive);
            console.log(out);
            if (typeof callback === "function") callback(null, out);
        });
    });
}

const matchTempNamesValues = function (names, values, active) {
    var pairs = [];
    for (var i = 0; i < names.length; i++) {
        if (active[i]) {
            pairs.push({
                name: names[i],
                value: values[i] / 100
            });
        }
    }
    return pairs;
}

module.exports = {
    poll: poll
}