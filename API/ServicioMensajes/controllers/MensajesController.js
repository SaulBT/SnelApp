const {response} = require('express');
const Chat = require('../models/Chat');
const {CrearChatAsync, CargarChatsAsync, CargarMensajesAsync, EnviarMensajeAsync} = require('../services/ServicioChat');

async function CrearChatControllerAsync(req, res = response, next) {
    try {
        const {alumnoId, instructorId} = req.params;
        
        const chatId = await CrearChatAsync(alumnoId, instructorId, Chat);
        
        return res.status(201).json(chatId);
    } catch(err) {
        next(err);
    }
}

async function CargarChatsControllerAsync(req, res = response, next) {
    try {
        const {usuarioId} = req.params;
        const {tipoUsuario} = req.body;

        const chats = await CargarChatsAsync(usuarioId, tipoUsuario, Chat);

        res.status(200).json(chats);
    } catch(err) {
        next(err);
    }
}

async function CargarMensajesControllerAsync(req, res = response, next) {
    try{
        const {chatId} = req.params;
        
        const mensajes = await CargarMensajesAsync(chatId, Chat);

        res.status(200).json(mensajes);
    } catch(err) {
        next(err);
    }
}

async function EnviarMensajeControllerAsync(req, res = response, next) {
    try {
        const {chatId} = req.params;
        const {texto, tipoUsuario} = req.body;

        const mensaje = await EnviarMensajeAsync(chatId, texto, tipoUsuario, Chat);

        res.status(201).json(mensaje);
    } catch(err) {
        next(err);
    }
}

module.exports = {CrearChatControllerAsync, CargarChatsControllerAsync, CargarMensajesControllerAsync, EnviarMensajeControllerAsync};