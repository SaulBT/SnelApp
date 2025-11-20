const {response} = require('express');
const Chat = require('../models/Chat');
const {CrearChatAsync, CargarChatsAsync, CargarMensajesAsync, EnviarMensajeAsync} = require('../services/ServicioChat');
const {ValidarId, ValidarTipoUsuario} = require('../validations/ValidacionesGenerales');

async function CrearChatControllerAsync(req, res = response, next) {
    try {
        const {alumnoId, instructorId} = req.params;
        ValidarId(alumnoId, "Alumno");
        ValidarId(instructorId, "Instructor");

        const chatId = await CrearChatAsync(alumnoId, instructorId, Chat);
        
        return res.status(201).json(chatId);
    } catch(err) {
        next(err);
    }
}

async function CargarChatsControllerAsync(req, res = response, next) {
    try {
        const {usuarioId} = req.params;
        ValidarId (usuarioId, "Usuario");
        const {tipoUsuario} = req.body;
        ValidarTipoUsuario(tipoUsuario);

        const chats = await CargarChatsAsync(usuarioId, tipoUsuario, Chat);

        res.status(200).json(chats);
    } catch(err) {
        next(err);
    }
}

async function CargarMensajesControllerAsync(req, res = response, next) {
    try{
        const {chatId} = req.params;
        ValidarId(chatId, "Chat");
        
        const mensajes = await CargarMensajesAsync(chatId, Chat);

        res.status(200).json(mensajes);
    } catch(err) {
        next(err);
    }
}

async function EnviarMensajeControllerAsync(req, res = response, next) {
    try {
        const {chatId} = req.params;
        ValidarId(chatId, "Chat");
        const {texto, tipoUsuario} = req.body;
        ValidarTipoUsuario(tipoUsuario);

        const mensaje = await EnviarMensajeAsync(chatId, texto, tipoUsuario, Chat);

        res.status(201).json(mensaje);
    } catch(err) {
        next(err);
    }
}

module.exports = {CrearChatControllerAsync, CargarChatsControllerAsync, CargarMensajesControllerAsync, EnviarMensajeControllerAsync};