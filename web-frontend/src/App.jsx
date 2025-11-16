import { Routes, Route } from "react-router-dom";
import Header from "./components/Header";

// Páginas existentes
import Home from "./pages/Home";
import Equipos from "./pages/Equipos";
import Register from "./pages/Register";
import Login from "./pages/Login";

// Solicitudes (nuevas)
import CrearSolicitud from "./pages/CrearSolicitud";
// Estos componentes los creamos luego
import MisSolicitudes from "./pages/MisSolicitudes";
import SolicitudesAdmin from "./pages/SolicitudesAdmin";

function App() {
  return (
    <>
      <Header />
      <Routes>
        {/* Página principal */}
        <Route path="/" element={<Home />} />

        {/* Equipos */}
        <Route path="/equipos" element={<Equipos />} />

        {/* Auth */}
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />

        {/* Solicitudes - Usuario */}
        <Route path="/solicitudes/crear" element={<CrearSolicitud />} />
        <Route path="/solicitudes/mias" element={<MisSolicitudes />} />

        {/* Solicitudes - Administrador */}
        <Route path="/solicitudes/admin" element={<SolicitudesAdmin />} />

        {/* Página no encontrada (opcional)
        <Route path="*" element={<NotFound />} /> */}
      </Routes>
    </>
  );
}

export default App;
