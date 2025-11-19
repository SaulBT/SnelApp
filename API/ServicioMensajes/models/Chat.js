const mongoose = require('mongoose');

const MensajeSchema = new mongoose.Schema({
    Texto: String,
    Fecha: {
        type: Date,
        default: Date.now,
        required: true
    },
    Emisor: Number
})

const ChatSchema = new mongoose.Schema({
    Alumno: Number,
    Instructor: Number,
    Mensajes: {
        type: [MensajeSchema]
    }
})

module.exports = mongoose.model('Chat', ChatSchema);