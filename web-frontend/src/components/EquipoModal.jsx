import React, { useState, useEffect } from "react";
import "../styles/EquipoModal.css";

const EquipoModal = ({ show, onClose, onSave, initialData }) => {
  const [formData, setFormData] = useState({
    nombre: "",
    modelo: "",
    numeroSerie: "",
    fechaAdquisicion: "",
    estado: "Disponible",
    imagen: "",
  });

  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
    } else {
      setFormData({
        nombre: "",
        modelo: "",
        numeroSerie: "",
        fechaAdquisicion: "",
        estado: "Disponible",
        imagen: "",
      });
    }
  }, [initialData]);

  if (!show) return null;

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onSave(formData);
  };

  return (
    <div className="equipo-modal-overlay">
      <div className="equipo-modal-modern">
        <h2 className="equipo-title">
          {initialData ? "Editar Equipo Médico" : "Registrar Nuevo Equipo"}
        </h2>
        <p className="equipo-subtitle">
          {initialData
            ? "Actualiza los datos del equipo."
            : "Completa los campos para registrar un nuevo equipo."}
        </p>

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label>Nombre</label>
            <input
              name="nombre"
              value={formData.nombre}
              onChange={handleChange}
              required
              placeholder="Ej. Electrocardiógrafo"
            />
          </div>

          <div className="input-group">
            <label>Modelo</label>
            <input
              name="modelo"
              value={formData.modelo}
              onChange={handleChange}
              required
              placeholder="Ej. GE MAC 2000"
            />
          </div>

          <div className="input-group">
            <label>Número de serie</label>
            <input
              name="numeroSerie"
              value={formData.numeroSerie}
              onChange={handleChange}
              required
              placeholder="Ej. SN123456"
            />
          </div>

          <div className="input-group">
            <label>Fecha de adquisición</label>
            <input
              type="date"
              name="fechaAdquisicion"
              value={formData.fechaAdquisicion?.split("T")[0] || ""}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-group">
            <label>Estado</label>
            <select
              name="estado"
              value={formData.estado}
              onChange={handleChange}
            >
              <option value="Disponible">Disponible</option>
              <option value="Ocupado">Ocupado</option>
              <option value="Mantenimiento">Mantenimiento</option>
            </select>
          </div>

          <div className="input-group">
            <label>Imagen (URL)</label>
            <input
              name="imagen"
              value={formData.imagen}
              onChange={handleChange}
              placeholder="https://..."
            />
          </div>

          {formData.imagen && (
            <div className="image-preview">
              <img
                src={formData.imagen}
                alt="Vista previa del equipo"
                onError={(e) => (e.target.style.display = "none")}
              />
            </div>
          )}

          <div className="equipo-btn-container">
            <button type="submit" className="equipo-btn primary">
              Guardar
            </button>
            <button
              type="button"
              className="equipo-btn secondary"
              onClick={onClose}
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EquipoModal;
