import React, { useRef } from "react";
import "../Styles/Home.css";

export default function Home() {
  const videoRef = useRef(null);

  return (
    <div className="home-container">
      {/* Video Background */}
      <div className="video-bg">
        <video
          ref={videoRef}
          autoPlay
          muted
          loop
          playsInline
          className="background-video"
        >
          {/* 👇 Puedes cambiar la ruta o cargar un archivo desde el front */}
          <source src="/src/assets/videos/HomePage.mp4" type="video/mp4" />
          Tu navegador no soporta videos HTML5.
        </video>
      </div>

      {/* Overlay (oscurece un poco el video para que resalte el texto) */}
      <div className="overlay"></div>

      {/* Main content */}
      <div className="hero-content">
        <h2>Tu bienestar, nuestra prioridad</h2>
        <p>Cuidamos tu salud con tecnología médica confiable.</p>
        <button className="get-started-btn">Get Started</button>
      </div>
    </div>
  );
}
