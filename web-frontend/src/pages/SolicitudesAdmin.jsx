import { useEffect, useState } from "react";
import {
  getSolicitudesAdmin,
  aprobarSolicitud,
  rechazarSolicitud,
} from "../services/requestService";
import "../styles/AdminSolicitudes.css";

const SolicitudesAdmin = () => {
  const [solicitudes, setSolicitudes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // ============================
  //  CARGAR SOLICITUDES
  // ============================
  const cargarSolicitudes = async () => {
    try {
      const resp = await getSolicitudesAdmin();
      setSolicitudes(resp.data);
    } catch (err) {
      console.error("Error cargando solicitudes:", err);
      setError("No se pudieron cargar las solicitudes");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarSolicitudes();
  }, []);

  // ============================
  //  APROBAR / RECHAZAR
  // ============================
  const manejarAprobacion = async (id) => {
    try {
      await aprobarSolicitud(id);
      cargarSolicitudes(); // 🔄 refresca
    } catch (err) {
      console.error("Error aprobando:", err);
      alert("No se pudo aprobar la solicitud");
    }
  };

  const manejarRechazo = async (id) => {
    try {
      await rechazarSolicitud(id);
      cargarSolicitudes();
    } catch (err) {
      console.error("Error rechazando:", err);
      alert("No se pudo rechazar la solicitud");
    }
  };

  // ============================
  //  UI
  // ============================
  if (loading) return <h3 className="text-center mt-5">Cargando solicitudes...</h3>;
  if (error) return <h3 className="text-danger text-center mt-5">{error}</h3>;

  return (
    <div className="admin-bg">
      <div className="container">
        <h2 className="admin-title">Administrar Solicitudes</h2>

        {solicitudes.length === 0 ? (
          <p className="text-center text-light">No hay solicitudes registradas.</p>
        ) : (
          <div className="row">
            {solicitudes.map((s) => (
              <div key={s.id} className="col-md-6 mb-4">
                <div className="admin-card">
                  <div className="admin-card-body">

                    {/* ESTADO */}
                    <span
                      className={`estado-tag ${
                        s.estado === "Aprobada"
                          ? "aprobada"
                          : s.estado === "Rechazada"
                          ? "rechazada"
                          : "pendiente"
                      }`}
                    >
                      {s.estado}
                    </span>

                    <h4 className="admin-card-title mt-3">
                      {s.equipo?.nombre ?? "Equipo desconocido"}
                    </h4>

                    <p><strong>Usuario:</strong> {s.usuarioId}</p>
                    <p><strong>Procedimiento:</strong> {s.procedimiento}</p>
                    <p>
                      <strong>Fecha programada:</strong>{" "}
                      {new Date(s.fechaProgramada).toLocaleString()}
                    </p>

                    {s.observaciones && (
                      <p><strong>Observaciones:</strong> {s.observaciones}</p>
                    )}

                    <hr />

                    <p><strong>Marca:</strong> {s.equipo?.marca}</p>
                    <p><strong>Modelo:</strong> {s.equipo?.modelo}</p>

                    {/* BOTONES */}
                    {s.estado === "Pendiente" ? (
                      <div className="btn-group-admin mt-3">
                        <button
                          className="btn-aceptar"
                          onClick={() => manejarAprobacion(s.id)}
                        >
                          ✔ Aprobar
                        </button>

                        <button
                          className="btn-rechazar"
                          onClick={() => manejarRechazo(s.id)}
                        >
                          ✖ Rechazar
                        </button>
                      </div>
                    ) : (
                      <small className="text-info d-block mt-2">
                        Revisado por: {s.revisadoPor ?? "N/A"}
                      </small>
                    )}

                    <small className="text-light d-block mt-2">
                      Fecha solicitud:{" "}
                      {new Date(s.fechaSolicitud).toLocaleString()}
                    </small>

                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default SolicitudesAdmin;
