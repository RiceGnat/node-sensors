const http = require("http");
const sensors = require("./sensors");

const errHandler = error => ({ error: error });

http.createServer((req, res) => {
    Promise.all([
        sensors.getSpeedFanData()
        .catch(errHandler),
        sensors.getAISuite2Data()
        .catch(errHandler),
        sensors.getOpenHWMonitorData()
        .catch(errHandler),
        sensors.getiCUEData()
        .catch(errHandler)
    ])
    .then(results => {
        res.writeHead(200, { "Content-Type": "application/json" });
        res.end(JSON.stringify(results));
    });
}).listen(process.env.PORT | 8080);
