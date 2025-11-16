import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom";
import {
  getEquipos,
  createEquipo,
  updateEquipo,
  deleteEquipo,
} from "../services/equipService";
import EquipoModal from "../components/EquipoModal";
import "../styles/Equipos.css";

const Equipos = () => {
  const [equipos, setEquipos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editData, setEditData] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [filter, setFilter] = useState("Todos");
  const [toast, setToast] = useState({ visible: false, message: "", type: "" });

  useEffect(() => {
    fetchEquipos();
  }, []);

  const fetchEquipos = async () => {
    try {
      setLoading(true);
      const data = await getEquipos();
      setEquipos(data);
    } catch (error) {
      showToast("Error al cargar los equipos.", "error");
    } finally {
      setLoading(false);
    }
  };

  const showToast = (message, type = "success") => {
    setToast({ visible: true, message, type });
    setTimeout(() => setToast({ visible: false, message: "", type: "" }), 3500);
  };

  const closeToast = () => setToast({ visible: false, message: "", type: "" });

  const handleRegister = () => {
    setEditData(null);
    setShowModal(true);
  };

  const handleEdit = (equipo) => {
    setEditData(equipo);
    setShowModal(true);
  };

  const handleSave = async (formData) => {
    try {
      if (editData) {
        await updateEquipo(editData.id, formData);
        showToast("Equipo actualizado correctamente", "success");
      } else {
        await createEquipo(formData);
        showToast("Equipo registrado correctamente", "success");
      }
      setShowModal(false);
      fetchEquipos();
    } catch (error) {
      showToast("Error al guardar el equipo", "error");
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm("¿Estás seguro de eliminar este equipo?")) {
      try {
        await deleteEquipo(id);
        setEquipos(equipos.filter((eq) => eq.id !== id));
        showToast("Equipo eliminado correctamente", "warning");
      } catch (error) {
        showToast("Error al eliminar el equipo", "error");
      }
    }
  };

  const filteredEquipos = equipos.filter((eq) => {
    const matchesSearch =
      eq.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      eq.modelo.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesFilter =
      filter === "Todos" || eq.estado.toLowerCase() === filter.toLowerCase();
    return matchesSearch && matchesFilter;
  });

  if (loading) return <p className="loading">Cargando equipos...</p>;

  return (
    <>
      {/* === Toast Moderno Glass === */}
      {toast.visible &&
        ReactDOM.createPortal(
          <div className={`toast-glass ${toast.type}`}>
            <div className="toast-icon">
              {toast.type === "success" && (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="2"
                  stroke="currentColor"
                >
                  <path d="M20 6L9 17l-5-5" />
                </svg>
              )}
              {toast.type === "warning" && (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="2"
                  stroke="currentColor"
                >
                  <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                  <line x1="12" y1="9" x2="12" y2="13" />
                  <line x1="12" y1="17" x2="12.01" y2="17" />
                </svg>
              )}
              {toast.type === "info" && (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="2"
                  stroke="currentColor"
                >
                  <circle cx="12" cy="12" r="10" />
                  <line x1="12" y1="16" x2="12" y2="12" />
                  <line x1="12" y1="8" x2="12.01" y2="8" />
                </svg>
              )}
              {toast.type === "error" && (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="2"
                  stroke="currentColor"
                >
                  <circle cx="12" cy="12" r="10" />
                  <line x1="15" y1="9" x2="9" y2="15" />
                  <line x1="9" y1="9" x2="15" y2="15" />
                </svg>
              )}
            </div>
            <div className="toast-text">
              <strong>
                {toast.type === "success" && "Operación exitosa"}
                {toast.type === "warning" && "Advertencia"}
                {toast.type === "info" && "Información"}
                {toast.type === "error" && "Error"}
              </strong>
              <p>{toast.message}</p>
            </div>
            <button className="toast-close" onClick={closeToast}>
              ×
            </button>
          </div>,
          document.body
        )}

      {/* === Contenido Principal === */}
      <div className="equipos-page">
        <div className="equipos-wrapper-clean">
          <div className="equipos-container-clean">
            <div className="equipos-header">
              <h2 className="equipos-title">
                <span className="highlight">Equipos Médicos</span>
              </h2>
              <button className="btn-register" onClick={handleRegister}>
                + Registrar nuevo equipo
              </button>
            </div>

            {/* === Buscador === */}
            <div className="equipos-search-bar">
              <div className="equipos-search-container">
                <input
                  type="text"
                  className="equipos-search"
                  placeholder="Buscar equipo..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
                <select
                  className="equipos-filter"
                  value={filter}
                  onChange={(e) => setFilter(e.target.value)}
                >
                  <option value="Todos">Todos</option>
                  <option value="Disponible">Disponible</option>
                  <option value="Ocupado">Ocupado</option>
                  <option value="Mantenimiento">Mantenimiento</option>
                </select>
              </div>
            </div>

            {/* === Lista === */}
            <div className="equipos-list">
              {filteredEquipos.length > 0 ? (
                filteredEquipos.map((eq) => (
                  <div key={eq.id} className="equipo-card">
                    <div className="equipo-info">
                      <img
                        src={
                          eq.imagen && eq.imagen.trim() !== ""
                            ? eq.imagen
                            : "https://via.placeholder.com/80x80?text=Sin+imagen"
                        }
                        alt={eq.nombre}
                        className="equipo-img"
                      />
                      <div className="equipo-text">
                        <h4 className="equipo-nombre">{eq.nombre}</h4>
                        <p className="equipo-detalle">{eq.modelo}</p>
                      </div>
                    </div>

                    <div className="equipo-datos">
                      <p>
                        <strong>Serie:</strong> {eq.numeroSerie}
                      </p>
                      <p>
                        <strong>Adquisición:</strong>{" "}
                        {eq.fechaAdquisicion
                          ? new Date(eq.fechaAdquisicion).toLocaleDateString()
                          : "Sin fecha"}
                      </p>
                    </div>

                    <div className="equipo-estado">
                      <span
                        className={`estado-badge ${
                          eq.estado?.toLowerCase() === "disponible"
                            ? "verde"
                            : eq.estado?.toLowerCase().includes("mantenimiento")
                            ? "naranja"
                            : "rojo"
                        }`}
                      >
                        {eq.estado}
                      </span>
                    </div>

                    <div className="equipo-acciones">
                      <button
                        className="btn-editar"
                        onClick={() => handleEdit(eq)}
                      >
                        Editar
                      </button>
                      <button
                        className="btn-eliminar"
                        onClick={() => handleDelete(eq.id)}
                      >
                        Eliminar
                      </button>
                    </div>
                  </div>
                ))
              ) : (
                <p className="no-data">No hay equipos registrados.</p>
              )}
            </div>

            <EquipoModal
              show={showModal}
              onClose={() => setShowModal(false)}
              onSave={handleSave}
              initialData={editData}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default Equipos;
