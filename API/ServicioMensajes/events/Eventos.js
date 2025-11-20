const EventEmitter = require('events');

class Eventos extends EventEmitter {}

module.exports = new Eventos();