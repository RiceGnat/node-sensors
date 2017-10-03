const http = require("http");
const url = require("url");
const speedfan = require("./speedfan");

const server = http.createServer((req, res) => {
    if (url.parse(req.url, true).pathname.split("/")[1].toLowerCase() == "speedfan") {
        speedfan.poll((error, results) => {
            if (error) {
                res.writeHead(500);
                res.end("An error occurred getting SpeedFan data");
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
