function ManejadorErrores(err, req, res, next) {
    if (err.statusCode && err.mensaje) {
        return res.status(err.statusCode).json({ error: err.mensaje });
    }

    console.error('Error inesperado: ', err);
    return res.status(500).json({
        error: 'Error interno del servidor',
        detalle: err.message
    });
}

module.exports = ManejadorErrores;