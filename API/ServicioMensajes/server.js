const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const mongoose = require('mongoose');
require('dotenv').config();
const swaggerUi = require('swagger-ui-express');
const swaggerSpec = require('./configuration/SwaggerConfig');
const { enviarMensajeTiempoReal } = require('./services/ServicioTiempoReal');

const app = express();
app.use(express.json());

app.use('/chats', require('./routes/ChatRutas'));
app.use('/swagger', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

const server = http.createServer(app);
const io = new Server(server, {
  cors: { origin: '*' }, // CAMBIAR
});

// Socket.IO: conexión y join a salas - CAMBIAR
io.on('connection', (socket) => {
  console.log('Cliente conectado', socket.id);

  socket.on('joinChat', (chatId) => {
    socket.join(chatId);
    console.log(`Socket ${socket.id} se unió al chat ${chatId}`);
  });
});

// Aquí conectas eventos
enviarMensajeTiempoReal(io);

mongoose.connect(process.env.MONGODB_URI_DEVELOPMENT) // Cambia a MONGODB_URI_DEVELOPMENT si estás en desarrollo
  .then(() => console.log('Conectado a MongoDB'))
  .catch((err) => console.error('Error conectando a MongoDB:', err));

server.listen(process.env.PORT_DEVELOPMENT, () => { //Cambiar a PORT_DEVELOPMENT para desarrollo
  console.log('Servidor en puerto 5002');
});

module.exports = { app, server, io };