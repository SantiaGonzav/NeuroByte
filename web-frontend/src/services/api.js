import axios from "axios";

// 🔹 URL base de tu backend (.NET)
const api = axios.create({
  baseURL: "http://localhost:5110/api", // 👈 tu AuthService corre aquí
  headers: {
    "Content-Type": "application/json",
  },
});

// 🔸 Interceptor para incluir el token JWT si existe
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
