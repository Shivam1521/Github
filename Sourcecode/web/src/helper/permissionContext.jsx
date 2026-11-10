import { createContext, useContext, useEffect, useState } from "react";
import RoleMasterService from "../services/RoleMasterService";

const PermissionContext = createContext();

export const usePermissions = () => useContext(PermissionContext);

export const PermissionProvider = ({ children }) => {
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);
  const roleId = sessionStorage.getItem("roleId");

  const refreshPermissions = async () => {
    setLoading(true);
    try {
      const service = new RoleMasterService();
      if (roleId) {
        const res = await service.GetPages(roleId);
        const allPages = res?.responseData;
        setPermissions(allPages || []);
      } else {
        setPermissions([]);
      }
    } catch (err) {
      console.error("Failed to fetch permissions", err);
      setPermissions([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    refreshPermissions();
  }, [roleId]);

  return (
    <PermissionContext.Provider
      value={{
        permissions,
        refreshPermissions,
        loading,
      }}
    >
      {children}
    </PermissionContext.Provider>
  );
};
