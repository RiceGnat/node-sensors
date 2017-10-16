const http = require("http");
const url = require("url");
const speedfan = require("./sensor");

const server = http.createServer((req, res) => {
    var reqUrl = url.parse(req.url, true);
    if (reqUrl.pathname.split("/")[1].toLowerCase() == "speedfan") {
        var options = reqUrl.query;
        speedfan.pollSpeedFan(options.all, (error, results) => {
            if (error) {
                res.writeHead(500);
                res.end("An error occurred getting SpeedFan data. SpeedFan may not be running!");
            }
            else {
                res.writeHead(200, { "Content-Type": "text/json" });
                res.end(JSON.stringify(results));
            }
        });
    }
    else if (reqUrl.pathname.split("/")[1].toLowerCase() == "aisuite") {
        speedfan.pollAISuite((error, results) => {
            if (error) {
                res.writeHead(500);
                res.end("An error occurred getting AI Suite data. AI Suite may not be running or the process may not have sufficient privileges.");
            }
            else {
                res.writeHead(200, { "Content-Type": "text/json" });
                res.end(JSON.stringify(results));
            }
        });
    }
    else {
        res.writeHead(404);
        res.end();
    } 
}).listen(process.env.PORT | 8080);
