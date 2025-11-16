import axios from "axios";

const axiosInstance = axios.create({
  baseURL: "http://localhost:5294/api", // 👈 PUERTO CORRECTO DE REQUESTSERVICE
  withCredentials: false,               // Evita problemas CORS si no usas cookies
});

// Interceptor para agregar token JWT automáticamente
axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export default axiosInstance;
