const http = require("http");
const url = require("url");
const sensor = require("./sensor");

const errHandler = error => ({ error: error });

const server = http.createServer((req, res) => {
    Promise.all([
        sensor.getSpeedFanData()
        .then(results => ({
            source: "SpeedFan",
            data: results
        }), errHandler),
        sensor.getAISuite2Data()
        .then(results => ({
            source: "AI Suite II",
            data: results
        }), errHandler)
    ])
    .then(results => {
        res.writeHead(200, { "Content-Type": "application/json" });
        res.end(JSON.stringify(results));
    });
}).listen(process.env.PORT | 8080);
