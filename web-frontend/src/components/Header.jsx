import { Link, useLocation } from "react-router-dom";
import "../styles/Header.css"; // 👈 asegúrate que sea minúscula si tu carpeta se llama "styles"

function Header() {
  const location = useLocation();
  const isHome = location.pathname === "/";

  return (
    <nav
      className={`navbar navbar-expand-lg fixed-top ${
        isHome ? "navbar-transparent" : "navbar-dark bg-dark"
      }`}
    >
      <div className="container-fluid">
        {/* Logo o nombre de marca */}
        <Link className="navbar-brand fw-bold" to="/">
          NeuroByte
        </Link>

        {/* Menú de navegación */}
        <div className="collapse navbar-collapse justify-content-end">
          <ul className="navbar-nav">
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

            <li className="nav-item">
              <Link
                className={`nav-link ${
                  location.pathname === "/sobre-nosotros" ? "active-link" : ""
                }`}
                to="/sobre-nosotros"
              >
                Sobre nosotros
              </Link>
            </li>

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
                to="/register" // 👈 corregido, antes decía "/registro"
              >
                Registrarse
              </Link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Header;
