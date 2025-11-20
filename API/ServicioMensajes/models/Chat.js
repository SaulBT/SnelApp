const mongoose = require('mongoose');
const AutoIncrement = require('mongoose-sequence')(mongoose);

const MensajeSchema = new mongoose.Schema({
    Texto: String,
    Fecha: {
        type: Date,
        default: Date.now,
        required: true
    },
    TipoUsuario: {
        type: String,
        enum: ['alumno', 'instructor'],
        required:true
    },
})

const ChatSchema = new mongoose.Schema({
    ChatId: {
        type: Number,
        unique: true,
    },
    AlumnoId: Number,
    InstructorId: Number,
    Mensajes: {
        type: [MensajeSchema]
    }
})

ChatSchema.plugin(AutoIncrement, {
    inc_field: 'ChatId',
    start_seq: 1,
});

module.exports = mongoose.model('Chat', ChatSchema);