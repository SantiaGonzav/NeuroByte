import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { AuthContext } from "../context/AuthContext";   // <-- IMPORTANTE
import "../styles/register.css";
import videoBg from "../assets/videos/HomePage.mp4";

export default function Login() {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext); // <-- AQUÍ TRAEMOS login()
  
  const [form, setForm] = useState({ email: "", password: "" });
  const [show, setShow] = useState(false);
  const [loading, setLoading] = useState(false);
  const [msg, setMsg] = useState(null);
  const [resetMode, setResetMode] = useState(false);
  const [resetEmail, setResetEmail] = useState("");
  const [resetMsg, setResetMsg] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const submit = async (e) => {
    e.preventDefault();

    if (!form.email.trim() || !form.password.trim()) {
      setMsg({ type: "error", text: "Correo y contraseña son obligatorios." });
      return;
    }

    setLoading(true);
    setMsg(null);

    try {
      // 👇 Backend espera password, NO passwordHash
      const payload = {
        email: form.email,
        password: form.password,
      };

      const res = await api.post("/auth/login", payload);

      // 🔥 AHORA SE USA el AuthContext.login()
      login(res.data);

      setMsg({ type: "success", text: "Inicio de sesión exitoso 🎉" });

      setTimeout(() => navigate("/"), 1200);
    } catch (err) {
      console.error("Error en login:", err);
      setMsg({
        type: "error",
        text: err.response?.data || "Credenciales incorrectas.",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();

    if (!resetEmail.trim()) {
      setResetMsg({ type: "error", text: "Ingresa tu correo." });
      return;
    }

    setResetMsg(null);
    setLoading(true);

    try {
      const res = await api.post("/auth/forgot-password", { email: resetEmail });
      setResetMsg({
        type: "success",
        text: res.data || "Te hemos enviado un correo 📩",
      });
    } catch (err) {
      console.error("Error en recuperación:", err);
      setResetMsg({
        type: "error",
        text: err.response?.data || "No se pudo enviar el correo.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="video-container">
      <video className="background-video" autoPlay loop muted playsInline>
        <source src={videoBg} type="video/mp4" />
      </video>

      <div className="reg-container">
        <div className="reg-card">
          {!resetMode ? (
            <>
              <h2>Iniciar sesión</h2>
              <p className="subtitle">
                Bienvenido de nuevo a <b>NeuroByte</b>
              </p>

              {msg && <div className={`msg ${msg.type}`}>{msg.text}</div>}

              <form onSubmit={submit}>
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
                    placeholder="Tu contraseña"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShow((prev) => !prev)}
                    className="show-btn"
                  >
                    {show ? "Ocultar" : "Mostrar"}
                  </button>
                </div>

                <button type="submit" disabled={loading}>
                  {loading ? "Ingresando..." : "Entrar"}
                </button>

                <button
                  type="button"
                  onClick={() => setResetMode(true)}
                  className="show-btn"
                  style={{ marginTop: "8px", background: "#0ea5e9" }}
                >
                  ¿Olvidaste tu contraseña?
                </button>
              </form>

              <p className="footer-text">
                ¿No tienes cuenta? <a href="/register">Regístrate</a>
              </p>
            </>
          ) : (
            <>
              <h2>Recuperar contraseña</h2>
              <p className="subtitle">
                Ingresa tu correo y te enviaremos una nueva contraseña temporal.
              </p>

              {resetMsg && (
                <div className={`msg ${resetMsg.type}`}>{resetMsg.text}</div>
              )}

              <form onSubmit={handleResetPassword}>
                <label>Correo</label>
                <input
                  type="email"
                  value={resetEmail}
                  onChange={(e) => setResetEmail(e.target.value)}
                  placeholder="tu@correo.com"
                  required
                />

                <button type="submit" disabled={loading}>
                  {loading ? "Enviando..." : "Enviar correo"}
                </button>

                <button
                  type="button"
                  onClick={() => setResetMode(false)}
                  className="show-btn"
                  style={{ marginTop: "8px", background: "#475569" }}
                >
                  Volver al login
                </button>
              </form>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
