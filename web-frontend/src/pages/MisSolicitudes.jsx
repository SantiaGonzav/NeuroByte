import { useEffect, useState } from "react";
import { getMisSolicitudes } from "../services/requestService";
import "../styles/MisSolicitudes.css";

const MisSolicitudes = () => {
  const [solicitudes, setSolicitudes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchSolicitudes = async () => {
      try {
        const resp = await getMisSolicitudes();
        setSolicitudes(resp.data);
      } catch (err) {
        console.error("Error cargando solicitudes:", err);
        setError("No se pudieron cargar las solicitudes.");
      } finally {
        setLoading(false);
      }
    };

    fetchSolicitudes();
  }, []);

  if (loading)
    return (
      <div className="mis-solicitudes-bg">
        <h3 className="text-center mt-4">Cargando solicitudes...</h3>
      </div>
    );

  if (error)
    return (
      <div className="mis-solicitudes-bg">
        <h3 className="text-center text-danger mt-4">{error}</h3>
      </div>
    );

  return (
    <div className="mis-solicitudes-bg">
      <h2 className="mis-titulo">Mis Solicitudes</h2>

      {solicitudes.length === 0 ? (
        <p className="text-center">Aún no has realizado ninguna solicitud.</p>
      ) : (
        <div className="container">
          <div className="row">
            {solicitudes.map((s) => (
              <div key={s.id} className="col-md-6 col-lg-4 mb-4">
                <div className="mis-card">

                  {/* ESTADO */}
                  <span
                    className={`mis-badge ${
                      s.estado === "Aprobada"
                        ? "aprobada"
                        : s.estado === "Rechazada"
                        ? "rechazada"
                        : "pendiente"
                    }`}
                  >
                    {s.estado}
                  </span>

                  {/* INFORMACIÓN */}
                  <h4 className="mt-3">Equipo: {s.equipo?.nombre}</h4>

                  <p>
                    <strong>Fecha programada:</strong>{" "}
                    {new Date(s.fechaProgramada).toLocaleString()}
                  </p>

                  <p>
                    <strong>Procedimiento:</strong> {s.procedimiento}
                  </p>

                  {s.observaciones && (
                    <p>
                      <strong>Observaciones:</strong> {s.observaciones}
                    </p>
                  )}

                  <hr />

                  <p>
                    <strong>Marca:</strong> {s.equipo?.marca}
                  </p>
                  <p>
                    <strong>Modelo:</strong> {s.equipo?.modelo}
                  </p>

                  <small className="text-muted">
                    Fecha de solicitud:{" "}
                    {new Date(s.fechaSolicitud).toLocaleString()}
                  </small>

                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default MisSolicitudes;
