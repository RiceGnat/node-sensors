const http = require("http");
const url = require("url");
const sensors = require("./sensors");

const errHandler = error => ({ error: error });

const server = http.createServer((req, res) => {
    Promise.all([
        sensors.getSpeedFanData()
        .then(results => ({
            source: "SpeedFan",
            data: results
        }), errHandler),
        sensors.getAISuite2Data()
        .then(results => ({
            source: "AI Suite II",
            data: results
        }), errHandler),
        sensors.getiCUEData()
        .then(results => ({
            source: "iCUE",
            data: results
        }), errHandler)
    ])
    .then(results => {
        res.writeHead(200, { "Content-Type": "application/json" });
        res.end(JSON.stringify(results));
    });
}).listen(process.env.PORT | 8080);
