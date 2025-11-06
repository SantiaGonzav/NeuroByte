import React, { useEffect, useState } from "react";
import { getEquipos } from "../services/equipService";
import "../styles/Equipos.css";

const Equipos = () => {
  const [equipos, setEquipos] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchEquipos = async () => {
      try {
        const data = await getEquipos();
        setEquipos(data);
      } catch (error) {
        console.error("Error al cargar los equipos", error);
      } finally {
        setLoading(false);
      }
    };
    fetchEquipos();
  }, []);

  const handleEdit = (id) => console.log("Editar equipo:", id);
  const handleDelete = (id) => console.log("Eliminar equipo:", id);

  if (loading) return <p className="loading">Cargando equipos...</p>;

  return (
    <div className="equipos-wrapper-clean">
      <div className="equipos-container-clean">
        <div className="equipos-header">
          <h2 className="equipos-title">Equipos Médicos</h2>
          <button className="btn-register">+ Registrar Nuevo Equipo</button>
        </div>

        <table className="equipos-table-clean">
          <thead>
            <tr>
              <th>ID</th>
              <th>Imagen</th>
              <th>Nombre</th>
              <th>Modelo</th>
              <th>Número de Serie</th>
              <th>Fecha de Adquisición</th>
              <th>Estado</th>
              <th>Acción</th>
            </tr>
          </thead>
          <tbody>
            {equipos.length > 0 ? (
              equipos.map((eq) => (
                <tr key={eq.id}>
                  <td>{eq.id}</td>
                  <td>
                    <img
                      src={
                        eq.imagen && eq.imagen.trim() !== ""
                          ? eq.imagen
                          : "https://via.placeholder.com/80x60?text=Sin+imagen"
                      }
                      alt={eq.nombre}
                      className="img-tabla"
                    />
                  </td>
                  <td>{eq.nombre}</td>
                  <td>{eq.modelo}</td>
                  <td>{eq.numeroSerie}</td>
                  <td>
                    {eq.fechaAdquisicion
                      ? new Date(eq.fechaAdquisicion).toLocaleDateString()
                      : "Sin fecha"}
                  </td>
                  <td>
                    <span
                      className={`estado ${
                        eq.estado?.toLowerCase() === "disponible"
                          ? "verde"
                          : eq.estado?.toLowerCase() === "mantenimiento"
                          ? "naranja"
                          : "rojo"
                      }`}
                    >
                      {eq.estado}
                    </span>
                  </td>
                  <td className="action-cell">
                    <div className="dropdown">
                      <button className="action-btn">Acción</button>
                      <div className="dropdown-menu">
                        <button onClick={() => handleEdit(eq.id)}>
                          Editar
                        </button>
                        <button
                          onClick={() => handleDelete(eq.id)}
                          className="delete"
                        >
                          Eliminar
                        </button>
                      </div>
                    </div>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="8" className="no-data">
                  No hay equipos registrados.
                </td>
              </tr>
            )}
          </tbody>
        </table>

        <p className="equipos-total">
          Total de equipos registrados: {equipos.length}
        </p>
      </div>
    </div>
  );
};

export default Equipos;
