const PROXY_CONFIG = [{
  context: [
    "/api",
    "/Temp",
    "/Upload",
  ],
  target: {
    "host": "localhost",
    "protocol": "https:",
    "port": 44321
  },
  secure: false,
  ws: true,
  logLevel: 'debug'
}];
module.exports = PROXY_CONFIG;
