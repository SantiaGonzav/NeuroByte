import axios from "./axiosInstance.js";

// ===============================
//  CREAR SOLICITUD
// ===============================
export const crearSolicitud = async (solicitud) => {
  const token = localStorage.getItem("token");
  return axios.post("/Solicitudes", solicitud, {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  OBTENER MIS SOLICITUDES
// ===============================
export const getMisSolicitudes = async () => {
  const token = localStorage.getItem("token");
  return axios.get("/Solicitudes/mias", {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  OBTENER TODAS LAS SOLICITUDES (ADMIN)
// ===============================
export const getSolicitudesAdmin = async () => {
  const token = localStorage.getItem("token");
  return axios.get("/Solicitudes", {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  OBTENER SOLICITUD POR ID
// ===============================
export const getSolicitudById = async (id) => {
  const token = localStorage.getItem("token");
  return axios.get(`/Solicitudes/${id}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  APROBAR SOLICITUD
// ===============================
export const aprobarSolicitud = async (id) => {
  const token = localStorage.getItem("token");
  return axios.put(`/Solicitudes/${id}/aprobar`, null, {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  RECHAZAR SOLICITUD
// ===============================
export const rechazarSolicitud = async (id) => {
  const token = localStorage.getItem("token");
  return axios.put(`/Solicitudes/${id}/rechazar`, null, {
    headers: { Authorization: `Bearer ${token}` }
  });
};

// ===============================
//  CONTAR PENDIENTES (ADMIN)
// ===============================
export const getSolicitudesPendientes = async () => {
  const token = localStorage.getItem("token");
  const resp = await axios.get("/Solicitudes", {
    headers: { Authorization: `Bearer ${token}` }
  });

  return resp.data.filter((s) => s.estado === "Pendiente").length;
};
