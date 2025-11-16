import axios from "axios";

const API_URL = "http://localhost:5110/api/auth";

/**
 * 📦 Registrar usuario (con imagen opcional)
 */
export const registerUser = async (userData) => {
  try {
    const formData = new FormData();
    formData.append("username", userData.username);
    formData.append("email", userData.email);
    formData.append("password", userData.password);
    if (userData.profileImage)
      formData.append("profileImage", userData.profileImage);

    const res = await axios.post(`${API_URL}/register`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    return res.data;
  } catch (error) {
    console.error("❌ Error al registrar usuario:", error);
    throw error.response?.data || { message: "Error al registrar usuario" };
  }
};

/**
 * 🔐 Iniciar sesión (JWT)
 */
export const loginUser = async (userData) => {
  try {
    const res = await axios.post(`${API_URL}/login`, {
      email: userData.email,
      password: userData.password, // 👈 CORREGIDO
    });

    // Guardar token y usuario
    if (res.data?.token) {
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("user", JSON.stringify(res.data.user));
    }

    return res.data;
  } catch (error) {
    console.error("❌ Error al iniciar sesión:", error);
    throw error.response?.data || { message: "Error al iniciar sesión" };
  }
};

/**
 * 🚪 Cerrar sesión
 */
export const logoutUser = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
};
