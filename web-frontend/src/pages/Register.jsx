import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/register.css";
import videoBg from "../assets/videos/HomePage.mp4"; // ✅ Import correcto del video

export default function Register() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    role: "User",
  });
  const [show, setShow] = useState(false);
  const [loading, setLoading] = useState(false);
  const [msg, setMsg] = useState(null);

  // 📥 Manejo de inputs
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  // 🚀 Envío del formulario
  const submit = async (e) => {
    e.preventDefault();

    if (!form.username || !form.email || !form.password) {
      setMsg({ type: "error", text: "Completa todos los campos" });
      return;
    }

    setLoading(true);
    setMsg(null);

    try {
      const payload = {
        username: form.username,
        email: form.email,
        passwordHash: form.password,
        role: form.role,
      };

      await api.post("/auth/register", payload);

      setMsg({ type: "success", text: "Registro exitoso 🎉 Redirigiendo..." });
      setTimeout(() => navigate("/login"), 1500);
    } catch (err) {
      console.error("Error en registro:", err);
      setMsg({
        type: "error",
        text: err.response?.data || "Error en el servidor",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="video-container">
      {/* 🎥 Fondo de video */}
      <video className="background-video" autoPlay loop muted playsInline>
        <source src={videoBg} type="video/mp4" />
        Tu navegador no soporta videos HTML5.
      </video>

      {/* Contenido del formulario */}
      <div className="reg-container">
        <div className="reg-card">
          <h2>Crear cuenta</h2>
          <p className="subtitle">
            Regístrate y comienza a usar <b>NeuroByte</b>
          </p>

          {msg && <div className={`msg ${msg.type}`}>{msg.text}</div>}

          <form onSubmit={submit}>
            <label>Usuario</label>
            <input
              name="username"
              value={form.username}
              onChange={handleChange}
              placeholder="Tu nombre de usuario"
              required
            />

            <label>Correo</label>
            <input
              name="email"
              type="email"
              value={form.email}
              onChange={handleChange}
              placeholder="tu@correo.com"
              required
            />

            <label>Contraseña</label>
            <div className="password-field">
              <input
                name="password"
                type={show ? "text" : "password"}
                value={form.password}
                onChange={handleChange}
                placeholder="Mínimo 6 caracteres"
                required
              />
              <button
                type="button"
                onClick={() => setShow((s) => !s)}
                className="show-btn"
              >
                {show ? "Ocultar" : "Mostrar"}
              </button>
            </div>

            <label>Rol</label>
            <div className="select-wrapper">
              <select
                name="role"
                value={form.role}
                onChange={handleChange}
                required
              >
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </div>

            <button type="submit" disabled={loading}>
              {loading ? "Registrando..." : "Registrar"}
            </button>
          </form>

          <p className="footer-text">
            ¿Ya tienes cuenta? <a href="/login">Inicia sesión</a>
          </p>
        </div>
      </div>
    </div>
  );
}
