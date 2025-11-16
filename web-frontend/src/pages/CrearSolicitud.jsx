import { useState, useEffect } from "react";
import { getEquipos } from "../services/equipService";
import { crearSolicitud } from "../services/requestService";
import "../styles/CrearSolicitud.css";

function CrearSolicitud() {
  const [equipos, setEquipos] = useState([]);
  const [selectedEquipo, setSelectedEquipo] = useState(null);

  const [estadoSolicitud, setEstadoSolicitud] = useState("");

  const [formData, setFormData] = useState({
    fechaProgramada: "",
    procedimiento: "",
    observaciones: ""
  });

  useEffect(() => {
    const fetchEquipos = async () => {
      const data = await getEquipos();
      setEquipos(data);
    };
    fetchEquipos();
  }, []);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  // ==================================
  // ENVIAR SOLICITUD
  // ==================================
  const handleSubmit = async (e) => {
    e.preventDefault();
    setEstadoSolicitud("procesando");

    try {
      await crearSolicitud({
        equipoId: selectedEquipo.id,
        fechaProgramada: formData.fechaProgramada,
        procedimiento: formData.procedimiento,
        observaciones: formData.observaciones
      });

      setEstadoSolicitud("registrada");
      setSelectedEquipo(null);

      setFormData({
        fechaProgramada: "",
        procedimiento: "",
        observaciones: ""
      });

      setTimeout(() => setEstadoSolicitud(""), 3000);

    } catch (error) {
      console.error("Error al crear solicitud", error);
      setEstadoSolicitud("error");

      setTimeout(() => setEstadoSolicitud(""), 3000);
    }
  };

  // ==================================
  // OBTENER URL DE IMAGEN (ABS o REL)
  // ==================================
  const getImagenUrl = (url) => {
    if (!url) return "/fallback.png";

    // Si es URL completa (http/https)
    if (url.startsWith("http")) return url;

    // Si fuera ruta relativa (no es tu caso, pero queda soportado)
    return `${import.meta.env.VITE_EQUIP_BASE_URL}${url}`;
  };

  return (
    <div className="crear-solicitud-container">
      <h1 className="titulo">Selecciona un equipo</h1>

      {/* ESTADOS */}
      {estadoSolicitud === "procesando" && (
        <div className="estado-box estado-procesando">
          ⏳ Procesando solicitud...
        </div>
      )}

      {estadoSolicitud === "registrada" && (
        <div className="estado-box estado-registrada">
          ✅ Solicitud registrada con éxito
        </div>
      )}

      {estadoSolicitud === "error" && (
        <div className="estado-box estado-error">
          ❌ Error al registrar la solicitud
        </div>
      )}

      {/* LISTA DE EQUIPOS */}
      <div className="equipos-grid">
        {equipos.map((e) => (
          <div key={e.id} className="equipo-card">

            {/* IMAGEN FINAL Y CORRECTA */}
            <img
              src={getImagenUrl(e.imagen)}   // 🟢 CORREGIDO
              alt={e.nombre}
              className="equipo-img"
              onError={(ev) => (ev.target.src = "/fallback.png")}
            />

            <div className="equipo-info">
              <h3>{e.nombre}</h3>
              <p><strong>Marca:</strong> {e.marca}</p>
              <p><strong>Modelo:</strong> {e.modelo}</p>
              <p><strong>Serie:</strong> {e.numeroSerie}</p>
            </div>

            <button
              className="btn-solicitar"
              onClick={() => setSelectedEquipo(e)}
            >
              Solicitar
            </button>
          </div>
        ))}
      </div>

      {/* FORMULARIO */}
      {selectedEquipo && (
        <div className="form-panel">
          <h2>Solicitar equipo: {selectedEquipo.nombre}</h2>

          <form onSubmit={handleSubmit}>
            
            <label>Fecha y hora programada</label>
            <input
              type="datetime-local"
              name="fechaProgramada"
              value={formData.fechaProgramada}
              onChange={handleChange}
              required
            />

            <label>Procedimiento</label>
            <input
              type="text"
              name="procedimiento"
              value={formData.procedimiento}
              onChange={handleChange}
              required
            />

            <label>Observaciones</label>
            <textarea
              name="observaciones"
              value={formData.observaciones}
              onChange={handleChange}
            />

            <button className="btn-enviar" type="submit">
              Enviar solicitud
            </button>

            <button
              className="btn-cancelar"
              type="button"
              onClick={() => setSelectedEquipo(null)}
            >
              Cancelar
            </button>

          </form>
        </div>
      )}
    </div>
  );
}

export default CrearSolicitud;
