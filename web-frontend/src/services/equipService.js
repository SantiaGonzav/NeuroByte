// src/services/equipService.js
import axios from "axios";

const API_URL = "http://localhost:5189/api/equipos";

// ✅ Obtener todos los equipos
export const getEquipos = async () => {
  const response = await axios.get(API_URL);
  return response.data;
};

// ✅ Obtener un equipo por ID
export const getEquipoById = async (id) => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

// ✅ Crear un nuevo equipo
export const createEquipo = async (equipo) => {
  const response = await axios.post(API_URL, equipo, {
    headers: { "Content-Type": "application/json" },
  });
  return response.data;
};

// ✅ Actualizar un equipo
export const updateEquipo = async (id, equipo) => {
  const response = await axios.put(`${API_URL}/${id}`, equipo, {
    headers: { "Content-Type": "application/json" },
  });
  return response.data;
};

// ✅ Eliminar un equipo
export const deleteEquipo = async (id) => {
  await axios.delete(`${API_URL}/${id}`);
};
