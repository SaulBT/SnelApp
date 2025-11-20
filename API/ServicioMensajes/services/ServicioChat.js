const eventos = require('../events/Eventos');

async function CrearChatAsync(alumnoId, instructorId, chat) {
    
    const chatNuevo = new chat({
        AlumnoId: alumnoId,
        InstructorId: instructorId
    });

    await chatNuevo.save();
    const chatCreado = await chat
        .findOne({ AlumnoId: alumnoId, InstructorId: instructorId },)
        .select('ChatId -_id')
        .lean();

    return {
        ChatId: chatCreado?.ChatId ?? 0
    };
}

async function CargarChatsAsync(usuarioId, tipoUsuario, chat) {
    const filtro =
        tipoUsuario === "alumno" 
        ? { AlumnoId: usuarioId } 
        : { InstructorId: usuarioId };
    
    const chats = await chat.find(
        filtro,
        {
            _id: 0,
            ChatId: 1,
            AlumnoId: 1,
            InstructorId: 1,
            Mensajes: { $slice: -1 },
        }
    ).lean();

    return chats.map((chat) => ({
        ChatId: chat.ChatId,
        AlumnoId: chat.AlumnoId,
        InstructorId: chat.InstructorId,
        UltimoMensaje: chat.Mensajes?.[0] ?? null,
    }));
}

async function CargarMensajesAsync(chatId, chat) {
    const chatEncontrado = await chat
        .findOne({ ChatId: chatId, },)
        .select('Mensajes -_id')
        .lean();
        
    return chatEncontrado?.Mensajes ?? [];
}

async function EnviarMensajeAsync(chatId, texto, tipoUsuario, chat) {
    const resultado = await chat.findOneAndUpdate(
        { ChatId: chatId },
        {
            $push: {
                Mensajes: {
                    Texto: texto,
                    TipoUsuario: tipoUsuario,
                    Fecha: new Date(),
                },
            },
        },
        { new: true }
    );

    if (!resultado) 
        throw new Error('Chat no encontrado');

    eventos.emit('MensajeEnviado', {
        chatId,
        mensaje: texto,
    });

    const mensajes = resultado.Mensajes;
    return mensajes[mensajes.length - 1];
}

module.exports = {CrearChatAsync, CargarChatsAsync, CargarMensajesAsync, EnviarMensajeAsync};