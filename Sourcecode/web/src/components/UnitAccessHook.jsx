import { useEffect, useState } from "react";
import { useSelector } from "react-redux";

const unitAccessHook = () => {
  const [isSuperAdmin, setIsSuperAdmin] = useState(false);

  const unitId = useSelector((state) => state.unit.selectedUnit);

  useEffect(() => {
    setIsSuperAdmin(JSON.parse(sessionStorage.getItem("isSA") || "false"));
  }, []);

  // const unitId = !isSuperAdmin ? selectedUnit?.toString() : null;

  return {
    isSuperAdmin,
    unitId,
  };
};

export default unitAccessHook;
