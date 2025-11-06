import axios from "axios";

const API_URL = "http://localhost:5189/api/equipos"; // Cambia si tu endpoint es diferente

export const getEquipos = async () => {
  try {
    const response = await axios.get(API_URL);
    return response.data;
  } catch (error) {
    console.error("Error al obtener los equipos:", error);
    throw error;
  }
};
