import { Routes, Route } from "react-router-dom";
import Header from "./components/Header";
import Home from "./pages/Home";
import Equipos from "./pages/Equipos";
import Register from "./pages/Register";
import Login from "./pages/Login";

function App() {
  return (
    <>
      <Header />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/equipos" element={<Equipos />} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        {/* <Route path="*" element={<NotFound />} /> */}
      </Routes>
    </>
  );
}

export default App;
