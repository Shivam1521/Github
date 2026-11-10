import { usePermissions } from "../helper/permissionContext";

const Restricted = ({ path, type = "", children }) => {
  const { permissions } = usePermissions();

  const allPages = permissions.flatMap((group) =>
    Array.isArray(group.children) ? group.children : []
  );

  const matched = allPages.find((p) => p.path === path);
  const isAllowed = matched?.[type];

  return isAllowed ? children : null;
};

export default Restricted;
