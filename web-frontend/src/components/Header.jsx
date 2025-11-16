import { Link, useLocation, useNavigate } from "react-router-dom";
import { useContext, useState, useEffect, useRef } from "react";
import { AuthContext } from "../context/AuthContext";

import { useSolicitudes } from "../context/SolicitudesContext";
import { getSolicitudesPendientes } from "../services/requestService";

import "../styles/Header.css";
import defaultAvatar from "../assets/default-avatar.jpg";

function Header() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useContext(AuthContext);

  const { pendingCount, updatePendingCount } = useSolicitudes();

  const [showMenu, setShowMenu] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef(null);

  const role = user?.role?.toLowerCase();
  const isHome = location.pathname === "/";

  // 🧩 Cerrar menú si hace click afuera
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
        setShowMenu(false);
      }
    };

    document.addEventListener("click", handleClickOutside);
    return () => document.removeEventListener("click", handleClickOutside);
  }, []);

  // 🔥 Obtener solicitudes pendientes si es admin (sin estado local)
  useEffect(() => {
    const fetchPending = async () => {
      try {
        if (role === "admin") {
          const count = await getSolicitudesPendientes();
          updatePendingCount(count); // ← Actualiza el contexto global
        }
      } catch (error) {
        console.error("Error obteniendo solicitudes pendientes:", error);
      }
    };

    fetchPending();
  }, [role, updatePendingCount]);

  const handleLogout = () => {
    logout();
    setShowMenu(false);
    navigate("/login");
  };

  const profileImg = user?.profileImagePath
    ? `http://localhost:5110${user.profileImagePath}`
    : defaultAvatar;

  return (
    <nav
      className={`navbar navbar-expand-lg fixed-top ${
        isHome ? "navbar-transparent" : "navbar-dark bg-dark"
      }`}
    >
      <div className="container-fluid">
        {/* Logo */}
        <Link className="navbar-brand fw-bold" to="/">
          NeuroByte
        </Link>

        {/* Menú responsive */}
        <button
          className="navbar-toggler"
          type="button"
          onClick={() => setMenuOpen(!menuOpen)}
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div
          className={`collapse navbar-collapse justify-content-end ${
            menuOpen ? "show" : ""
          }`}
        >
          <ul className="navbar-nav align-items-center">
            {/* Enlaces generales */}
            <li className="nav-item">
              <Link
                className={`nav-link ${
                  location.pathname === "/" ? "active-link" : ""
                }`}
                to="/"
              >
                Inicio
              </Link>
            </li>

            <li className="nav-item">
              <Link
                className={`nav-link ${
                  location.pathname === "/equipos" ? "active-link" : ""
                }`}
                to="/equipos"
              >
                Equipos
              </Link>
            </li>

            {/* Si NO está logueado */}
            {!user && (
              <>
                <li className="nav-item">
                  <Link
                    className={`nav-link ${
                      location.pathname === "/login" ? "active-link" : ""
                    }`}
                    to="/login"
                  >
                    Iniciar sesión
                  </Link>
                </li>

                <li className="nav-item">
                  <Link
                    className={`nav-link ${
                      location.pathname === "/register" ? "active-link" : ""
                    }`}
                    to="/register"
                  >
                    Registrarse
                  </Link>
                </li>
              </>
            )}

            {/* Si está logueado */}
            {user && (
              <>
                {/* ADMIN */}
                {role === "admin" && (
                  <li className="nav-item position-relative">
                    <Link
                      className={`nav-link ${
                        location.pathname === "/solicitudes/admin"
                          ? "active-link"
                          : ""
                      }`}
                      to="/solicitudes/admin"
                    >
                      Administrar solicitudes
                    </Link>

                    {/* Badge dinámico */}
                    {pendingCount > 0 && (
                      <span className="badge-notify">{pendingCount}</span>
                    )}
                  </li>
                )}

                {/* USER */}
                {role === "user" && (
                  <>
                    <li className="nav-item">
                      <Link
                        className={`nav-link ${
                          location.pathname === "/solicitudes/crear"
                            ? "active-link"
                            : ""
                        }`}
                        to="/solicitudes/crear"
                      >
                        Realizar solicitud
                      </Link>
                    </li>

                    <li className="nav-item">
                      <Link
                        className={`nav-link ${
                          location.pathname === "/solicitudes/mias"
                            ? "active-link"
                            : ""
                        }`}
                        to="/solicitudes/mias"
                      >
                        Mis solicitudes
                      </Link>
                    </li>
                  </>
                )}

                {/* Avatar + menú */}
                <li className="nav-item dropdown user-menu" ref={menuRef}>
                  <img
                    src={profileImg}
                    alt="Perfil"
                    className="profile-pic"
                    onClick={() => setShowMenu(!showMenu)}
                    onError={(e) => (e.target.src = defaultAvatar)}
                  />

                  {showMenu && (
                    <ul className="dropdown-menu show-menu">
                      <li className="dropdown-item text-center fw-bold">
                        {user.username}
                      </li>

                      <li>
                        <button
                          className="dropdown-item text-danger"
                          onClick={handleLogout}
                        >
                          Cerrar sesión
                        </button>
                      </li>
                    </ul>
                  )}
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Header;
