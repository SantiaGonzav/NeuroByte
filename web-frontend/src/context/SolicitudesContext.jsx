import { createContext, useContext, useState } from "react";

const SolicitudesContext = createContext();

export const SolicitudesProvider = ({ children }) => {
  const [pendingCount, setPendingCount] = useState(0);

  const updatePendingCount = (count) => {
    setPendingCount(count);
  };

  return (
    <SolicitudesContext.Provider value={{ pendingCount, updatePendingCount }}>
      {children}
    </SolicitudesContext.Provider>
  );
};

export const useSolicitudes = () => useContext(SolicitudesContext);
