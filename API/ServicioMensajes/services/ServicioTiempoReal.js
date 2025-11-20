const eventos = require('../events/Eventos');

function enviarMensajeTiempoReal(io) {
    eventos.on('MensajeEnviado', ({ chatId, mensaje }) => {
        io.to(chatId).emit('nuevoMensaje', mensaje);
    });
}

module.exports = { enviarMensajeTiempoReal };