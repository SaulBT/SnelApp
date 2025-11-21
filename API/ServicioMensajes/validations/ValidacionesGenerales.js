function ValidarId(id, objeto) {
    if (!id) {
        throw { statusCode: 400, mensaje: `La id del ${objeto} es nula.` };
    }
    if (id <= 0) {
        throw { statusCode: 400, mensaje: `La id '${id}' del ${objeto} es inválida.` };
    }
}

function ValidarTipoUsuario(tipoUsuario) {
    if (!tipoUsuario) {
        throw { statusCode: 400, mensaje: `El tipo de usuario es nulo.` };
    }
    if (tipoUsuario != "alumno" && tipoUsuario != "instructor") {
        throw { statusCode: 400, mensaje: `El tipo de usuario '${tipoUsuario}' es inválido.` };
    }
}

function ValidarChat(chat, idChat) {
    if (!chat) {
        throw { statusCode: 404, mensaje: `No existe ningún chat con la id '${idChat}'.` };
    }
}

module.exports = {ValidarId, ValidarTipoUsuario, ValidarChat}