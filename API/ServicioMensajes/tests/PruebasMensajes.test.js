const {
    CrearChatControllerAsync,
    CargarChatsControllerAsync,
    CargarMensajesControllerAsync,
    EnviarMensajeControllerAsync
} = require('../controllers/MensajesController');
const ServicioChat = require('../services/ServicioChat');

jest.mock('../services/ServicioChat');

//P-1 Iniciar chat
test('Un Alumno inicia un Chat con un Instructor', async () => {
    const chatIdMock = { ChatId: 193 };
    ServicioChat.CrearChatAsync.mockResolvedValue(chatIdMock);

    const req = {
        params: { alumnoId: 56, instructorId: 89 },
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CrearChatControllerAsync(req, res, next);

    expect(ServicioChat.CrearChatAsync).toHaveBeenCalledWith(
        56,
        89,
        expect.any(Function)
    );
    expect(res.status).toHaveBeenCalledWith(201);
    expect(res.json).toHaveBeenCalledWith(chatIdMock);
    expect(next).not.toHaveBeenCalledWith();
});

//P-2
test('Iniciar Chat inválido', async () => {
    const req = {
        params: { alumnoId: 0, instructorId: 89 },
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CrearChatControllerAsync(req, res, next);

    expect(next).toHaveBeenCalledWith(
        expect.objectContaining({
            statusCode: 400,
            mensaje: "La id del Alumno es nula.",
        })
    );
    expect(res.status).not.toHaveBeenCalled();
    expect(res.json).not.toHaveBeenCalled()
});

//P-3
test('Obtener Chats de Alumno', async () => {
    const chatsBd = [
        {
            ChatId: 12,
            AlumnoId: 22,
            InstructorId: 781,
            UltimoMensaje: {
                Texto: "Buenas noches. Ya, la acabo de abrir. Saludos",
                Fecha: "2025-11-20T05:13:56.899Z",
                TipoUsuario: "instructor",
                _id: "691ea39436d7e5a04276efbf"
            }
        },
        {
            ChatId: 45,
            AlumnoId: 33,
            InstructorId: 73,
            UltimoMensaje: {
                Texto: "Gracias maestra",
                Fecha: "2025-11-21T05:13:56.899Z",
                TipoUsuario: "alumno",
                _id: "691ea78136d7e5a04276efbf"
            }
        }
    ]
    ServicioChat.CargarChatsAsync.mockResolvedValue(chatsBd);

    const req = {
        params: { usuarioId: 33 },
        body: { tipoUsuario: "alumno" }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CargarChatsControllerAsync(req, res, next);

    expect(ServicioChat.CargarChatsAsync).toHaveBeenCalledWith(
        33,
        "alumno",
        expect.any(Function)
    );
    expect(res.status).toHaveBeenCalledWith(200);
    expect(res.json).toHaveBeenCalledWith(chatsBd);
    expect(next).not.toHaveBeenCalledWith();
});

//P-4 
test('Obtener Chats de Alumno inválido', async () => {
    const req = {
        params: { usuarioId: 33 },
        body: { tipoUsuario: "inscrito" }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CargarChatsControllerAsync(req, res, next);

    expect(next).toHaveBeenCalledWith(
        expect.objectContaining({
            statusCode: 400,
            mensaje: "El tipo de usuario 'inscrito' es inválido.",
        })
    );
    expect(res.status).not.toHaveBeenCalled();
    expect(res.json).not.toHaveBeenCalled()
});

//P-5
test('Obtener Mensajes de Chat', async () => {
    const mensajesBd = [
        {
            Texto: "Hola mundo",
            Fecha: "2025-11-20T05:11:57.636Z",
            TipoUsuario: "alumno",
            _id: "691ea31d36d7e5a04276efb8"
        },
        {
            Texto: "Maestro, buenas noches ¿Cómo está? Disculpe ¿Ya está abierta la actividad 5?",
            Fecha: "2025-11-20T05:13:06.663Z",
            TipoUsuario: "alumno",
            _id: "691ea36236d7e5a04276efbb"
        },
        {
            Texto: "Saúl, buenas noches. Ya, la acabo de abrir. Saludos",
            Fecha: "2025-11-20T05:13:56.899Z",
            TipoUsuario: "instructor",
            _id: "691ea39436d7e5a04276efbf"
        }
    ]
    ServicioChat.CargarMensajesAsync.mockResolvedValue(mensajesBd);

    const req = {
        params: { chatId: 60 }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CargarMensajesControllerAsync(req, res, next);

    expect(ServicioChat.CargarMensajesAsync).toHaveBeenCalledWith(
        60,
        expect.any(Function)
    );
    expect(res.status).toHaveBeenCalledWith(200);
    expect(res.json).toHaveBeenCalledWith(mensajesBd);
    expect(next).not.toHaveBeenCalledWith();
});

//P-6
test('Obtener Mensajes de un Chat inválido', async () => {
    const req = {
        params: { chatId: -5 },
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await CargarMensajesControllerAsync(req, res, next);

    expect(next).toHaveBeenCalledWith(
        expect.objectContaining({
            statusCode: 400,
            mensaje: "La id '-5' del Chat es inválida.",
        })
    );
    expect(res.status).not.toHaveBeenCalled();
    expect(res.json).not.toHaveBeenCalled()
});

//P-7
test('Enviar Mensaje', async () => {
    const mensajeMock = {
        Texto: "Podrías decirle a tus compañeros que la actividad ya está abierta?",
        Fecha: "2025-11-21T20:04:57.993Z",
        TipoUsuario: "instructor",
        _id: "6920c5e96f4f07e3ca022f07"
    }
    ServicioChat.EnviarMensajeAsync.mockResolvedValue(mensajeMock);

    const req = {
        params: { chatId: 60 },
        body: {
            texto: "Podrías decirle a tus compañeros que la actividad ya está abierta?",
            tipoUsuario: "instructor"
        }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await EnviarMensajeControllerAsync(req, res, next);

    expect(ServicioChat.EnviarMensajeAsync).toHaveBeenCalledWith(
        60,
        "Podrías decirle a tus compañeros que la actividad ya está abierta?",
        "instructor",
        expect.any(Function)
    );
    expect(res.status).toHaveBeenCalledWith(201);
    expect(res.json).toHaveBeenCalledWith(mensajeMock);
    expect(next).not.toHaveBeenCalledWith();
});

//P-8
test('Enviar Mensaje id inválida', async () => {
    const req = {
        params: { chatId: 0 },
        body: {
            texto: "Podrías decirle a tus compañeros que la actividad ya está abierta?",
            tipoUsuario: "instructor"
        }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await EnviarMensajeControllerAsync(req, res, next);

    expect(next).toHaveBeenCalledWith(
        expect.objectContaining({
            statusCode: 400,
            mensaje: "La id del Chat es nula.",
        })
    );
    expect(res.status).not.toHaveBeenCalled();
    expect(res.json).not.toHaveBeenCalled()
});

//P-9
test('Enviar Mensaje a Chat inexistente', async () => {
    const errorMock = {
        statusCode: 404,
        mensaje: "No existe ningún chat con la id '723'."
    }
    ServicioChat.EnviarMensajeAsync.mockRejectedValue(errorMock);
    
    const req = {
        params: { chatId: 723 },
        body: {
            texto: "Podrías decirle a tus compañeros que la actividad ya está abierta?",
            tipoUsuario: "instructor"
        }
    };
    const res = {
        status: jest.fn().mockReturnThis(),
        json: jest.fn(),
    };
    const next = jest.fn();

    await EnviarMensajeControllerAsync(req, res, next);

    expect(next).toHaveBeenCalledWith(
        expect.objectContaining({
            statusCode: 404,
            mensaje: "No existe ningún chat con la id '723'.",
        })
    );
    expect(res.status).not.toHaveBeenCalled();
    expect(res.json).not.toHaveBeenCalled()
});