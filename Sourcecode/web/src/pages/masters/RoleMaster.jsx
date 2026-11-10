import React, { useCallback, useEffect, useState } from "react";
import {
  Box,
  Button,
  Typography,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Checkbox,
  IconButton,
  Collapse,
  CircularProgress,
} from "@mui/material";
import { DataGrid, getGridStringOperators } from "@mui/x-data-grid";
import EditIcon from "@mui/icons-material/Edit";
import SettingIcon from "@mui/icons-material/Settings";
import { KeyboardArrowDown, KeyboardArrowUp } from "@mui/icons-material";
import AddRoleModal from "./AddRoleModal";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchPages,
  fetchRoles,
  resetPermissions,
  savePermissions,
} from "../../store/roleMasterSlice";
import toast from "react-hot-toast";
import { usePermissions } from "../../helper/permissionContext";
import Restricted from "../../components/Restricted";
import { GridToolbar } from "@mui/x-data-grid/internals";
import unitAccessHook from "../../components/UnitAccessHook";

const permissions = [
  { name: "View", key: "isView" },
  { name: "Add", key: "isAdd" },
  { name: "Update", key: "isEdit" },
];

export const RoleMaster = () => {
  const { datasource, totalRecords, loading, error, pages, pageLoading } =
    useSelector((state) => state.roleMaster);
  const { unitId } = unitAccessHook();
  const initialData = !pageLoading ? pages : null;
  const [openGroups, setOpenGroups] = useState(() => {
    const expanded = {};
    initialData.forEach((group) => {
      expanded[group.group] = true;
    });
    return expanded;
  });

  const [open, setOpen] = useState(false);
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "CreateDate",
    sortOrder: "DESC",
    unitId: unitId || null,
  });
  const dispatch = useDispatch();

  const [selectedRow, setSelectedRow] = useState({});
  const [isEditMode, setIsEditMode] = useState(false);
  const [permissionsState, setPermissionsState] = useState([]);
  const [roleId, setRoleId] = useState(null);
  const { refreshPermissions } = usePermissions();
  const { saveLoading, permissionError, permissionSuccess } = useSelector(
    (state) => state.roleMaster,
  );

  useEffect(() => {
    if (roleId) {
      setPermissionsState(pages);
    }
  }, [pages]);

  useEffect(() => {
    if (permissionSuccess) {
      toast.success(permissionSuccess);
      dispatch(fetchPages(roleId));
      if (roleId == sessionStorage.getItem("roleId")) {
        refreshPermissions();
      }
      dispatch(resetPermissions());
    }
    if (permissionError) {
      toast.error(permissionError);
      dispatch(resetPermissions());
    }
    return () => {};
  }, [permissionSuccess, permissionError]);

  const handleSortModelChange = (newSortModel) => {
    if (newSortModel.length > 0) {
      const sortField = newSortModel[0].field;
      const sortDirection = newSortModel[0].sort;
      const updatedRequest = {
        ...tableRequest,
        sortColumn: sortField,
        sortOrder: sortDirection.toUpperCase(),
        pageNo: page + 1,
        pageSize,
      };
      setTableRequest(updatedRequest);
      dispatch(fetchRoles(updatedRequest));
    }
  };
  const handleCheckboxChange =
    (groupIndex, childIndex, permission) => (event) => {
      const updated = permissionsState.map((group, gIndex) => {
        if (gIndex !== groupIndex) return group;
        return {
          ...group,
          children: group.children.map((child, cIndex) =>
            cIndex === childIndex
              ? { ...child, [permission]: event.target.checked }
              : child,
          ),
        };
      });

      setPermissionsState(updated);
    };

  const toggleGroup = (groupName) => {
    setOpenGroups((prev) => ({ ...prev, [groupName]: !prev[groupName] }));
  };
  const getAllPermissions = useCallback(
    (permission) =>
      permissionsState?.flatMap((group) =>
        (group.children ?? [])
          ?.filter((child) => child?.[`${permission}Available`] !== false)
          .map((child) => child[permission]),
      ) || [],
    [permissionsState],
  );

  const isAllChecked = useCallback(
    (permission) => {
      const values = getAllPermissions(permission);
      return values.length > 0 && values.every((val) => val === true);
    },
    [getAllPermissions],
  );

  const isSomeChecked = useCallback(
    (permission) => {
      const values = getAllPermissions(permission);
      return values.some((val) => val === true);
    },
    [getAllPermissions],
  );
  const handleSelectAll = (permission) => (event) => {
    const checked = event.target.checked;
    const updated = permissionsState.map((group) => ({
      ...group,
      children: (group.children ?? []).map((child) => ({
        ...child,
        [permission]:
          child?.[`${permission}Available`] === false
            ? child[permission]
            : checked,
      })),
    }));
    setPermissionsState(updated);
  };

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      const newRequest = {
        ...tableRequest,
        unitId: String(unitId),
        pageNo: page + 1,
        pageSize: pageSize,
      };
      dispatch(fetchRoles(newRequest));
    } else {
      navigate("/login");
    }
  }, [page, pageSize, tableRequest, unitId]);

  const transformPayload = (data) => {
    const result = [];
    data.forEach((group) => {
      if (Array.isArray(group.children)) {
        group.children.forEach((child) => {
          result.push({
            pageId: child.pageId,
            roleId: roleId,
            isView: !!child.isView,
            isAdd: !!child.isAdd,
            isEdit: !!child.isEdit,
            createdBy: sessionStorage.getItem("userId"),
          });
        });
      }
    });
    return result;
  };

  const handleSave = async () => {
    const payload = transformPayload(permissionsState);
    dispatch(savePermissions(payload));
  };

  const columns = [
    {
      field: "unitName",
      headerName: "Unit Name",
      flex: 1,
    },
    {
      field: "roleName",
      headerName: "Role Name",
      flex: 1,
    },
    {
      field: "roleDesc",
      headerName: "Role Description",
      flex: 1,
    },
    {
      field: "userType",
      headerName: "Employee Type",
      flex: 1,
    },
    {
      field: "isActive",
      headerName: "Active",
      flex: 0.5,
      sortable: false,
      renderCell: (params) => (
        <input type="checkbox" checked={params.value} readOnly />
      ),
    },
    {
      field: "actions",
      headerName: "Action",
      flex: 0.8,
      sortable: false,
      renderCell: (params) => (
        <>
          <Restricted path="masters/role-master" type="isEdit">
            <span style={{ color: "green", cursor: "pointer", marginRight: 8 }}>
              <EditIcon
                sx={{ mt: 1.5 }}
                onClick={() => {
                  setOpen(true);
                  setSelectedRow(params.row);
                  setIsEditMode(true);
                }}
              />
            </span>

            <span style={{ cursor: "pointer" }}>
              <SettingIcon
                onClick={() => {
                  const roleId = params.row.roleId;
                  setTimeout(() => dispatch(fetchPages(roleId)), 0);
                  setRoleId(roleId);
                }}
              />
            </span>
          </Restricted>
        </>
      ),
    },
  ];

  function CustomNoRowsOverlay() {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100%",
          fontSize: "30px",
          fontWeight: "500",
        }}
      >
        <Typography
          variant="subtitle1"
          sx={{ fontWeight: "bold", fontSize: "30px" }}
        >
          No data found
        </Typography>
      </Box>
    );
  }

  const onFilterModelChange = (newFilterModel) => {
    if (newFilterModel.quickFilterValues?.length > 0) {
      setTableRequest((prev) => ({
        ...prev,
        searchValue: newFilterModel.quickFilterValues.join(" "),
      }));
    } else {
      setTableRequest((prev) => ({
        ...prev,
        searchValue: "",
      }));
    }
  };

  return (
    <Box sx={{ mb: 1 }}>
      <Box
        sx={{
          display: "flex",
          gap: 1.5,
        }}
      >
        <Typography
          variant="h5"
          sx={{
            fontWeight: 600,
            color: "#333",
            letterSpacing: "0.5px",
            transition: "color 0.3s",
          }}
        >
          Role Master
        </Typography>
      </Box>
      <hr />
      <Box>
        <Box>
          <Restricted path="masters/role-master" type="isAdd">
            <Button
              sx={{
                backgroundColor: "#1E2733",
                color: "#fff",
                my: 2,
                borderRadius: "8px",
                border: "2px solid #A0AEC0",
                fontWeight: 600,
                textTransform: "none",
                transition: "all 0.3s ease",
                "&:hover": {
                  backgroundColor: "#2A3442",
                  color: "#fff",
                  borderColor: "#fff",
                },
              }}
              onClick={() => {
                setOpen(true);
                setIsEditMode(false);
              }}
            >
              + Add
            </Button>
          </Restricted>
        </Box>
        <Grid container spacing={2} sx={{ pb: 2 }}>
          <Grid size={{ md: 6, xs: 12 }}>
            <Box sx={{ minHeight: "auto", width: "100%" }}>
              <DataGrid
                rows={datasource}
                getRowId={(row) => row.roleId}
                columns={columns}
                pagination
                paginationMode="server"
                sortingMode="server"
                slotProps={{
                  toolbar: {
                    printOptions: { disableToolbarButton: true },
                    csvOptions: { disableToolbarButton: true },
                    excelOptions: { disableToolbarButton: true },
                    showQuickFilter: true,
                    quickFilterProps: { debounceMs: 500 },
                  },
                }}
                initialState={{
                  sorting: {
                    sortModel: [{ field: "createDate", sort: "desc" }],
                  },
                }}
                onFilterModelChange={onFilterModelChange}
                showToolbar
                onSortModelChange={handleSortModelChange}
                rowCount={totalRecords}
                pageSizeOptions={[5, 10, 25, 100]}
                paginationModel={{ page, pageSize }}
                onPaginationModelChange={(model) => {
                  setPage(model.page);
                  setPageSize(model.pageSize);
                }}
                loading={loading}
                slots={{
                  toolbar: GridToolbar,
                  noRowsOverlay: CustomNoRowsOverlay,
                }}
                disableRowSelectionOnClick
                disableColumnSelector
                disableColumnMenu
                disableColumnFilter
                // disableColumnResize
                sx={{
                  ".MuiDataGrid-cell:focus, .MuiDataGrid-cell:focus-within": {
                    outline: "none",
                  },
                  ".MuiDataGrid-columnHeader:focus, .MuiDataGrid-columnHeader:focus-within":
                    {
                      outline: "none",
                    },
                }}
              />
            </Box>
          </Grid>
          <Grid size={{ md: 6, sx: 12 }} position="relative">
            {pageLoading ? (
              <Box
                sx={{
                  position: "absolute",
                  top: "50%",
                  left: "50%",
                  transform: "translate(-50%, -50%)",
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                }}
              >
                <CircularProgress size={60} />
              </Box>
            ) : permissionsState.length > 0 ? (
              <>
                <TableContainer sx={{ border: "1px solid #e6e6e6" }}>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell sx={{ fontWeight: "bold" }}>
                          Menu Name
                        </TableCell>
                        {permissions.map((perm) => (
                          <TableCell
                            key={perm.key}
                            align="center"
                            sx={{ fontWeight: "bold", width: "150px" }}
                          >
                            {perm.name}
                            <Checkbox
                              checked={isAllChecked(perm.key)}
                              indeterminate={
                                isSomeChecked(perm.key) &&
                                !isAllChecked(perm.key)
                              }
                              onChange={handleSelectAll(perm.key)}
                            />
                          </TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {permissionsState.map((group, groupIndex) => (
                        <React.Fragment key={group.group}>
                          <TableRow>
                            <TableCell colSpan={4}>
                              <IconButton
                                size="small"
                                onClick={() => toggleGroup(group.group)}
                              >
                                {openGroups[group.group] ? (
                                  <KeyboardArrowUp />
                                ) : (
                                  <KeyboardArrowDown />
                                )}
                              </IconButton>
                              <strong>{group.group}</strong>
                            </TableCell>
                          </TableRow>
                          <TableRow>
                            <TableCell colSpan={4} sx={{ p: 0 }}>
                              <Collapse
                                in={openGroups[group.group]}
                                timeout="auto"
                                unmountOnExit
                              >
                                <Box margin={1}>
                                  <Table size="small">
                                    <TableBody>
                                      {group?.children?.map(
                                        (child, childIndex) => (
                                          <TableRow key={child.pageId}>
                                            <TableCell sx={{ width: "120px" }}>
                                              {child.pageName}
                                            </TableCell>
                                            {permissions.map((perm) => {
                                              const availableKey = `${perm.key}Available`;
                                              return (
                                                <TableCell
                                                  align="right"
                                                  key={perm.key}
                                                  sx={{ width: "150px" }}
                                                >
                                                  <Checkbox
                                                    checked={child[perm.key]}
                                                    disabled={
                                                      child[availableKey] ===
                                                      false
                                                    }
                                                    onChange={handleCheckboxChange(
                                                      groupIndex,
                                                      childIndex,
                                                      perm.key,
                                                    )}
                                                    color="primary"
                                                  />
                                                </TableCell>
                                              );
                                            })}
                                          </TableRow>
                                        ),
                                      )}
                                    </TableBody>
                                  </Table>
                                </Box>
                              </Collapse>
                            </TableCell>
                          </TableRow>
                        </React.Fragment>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>

                <Box sx={{ display: "flex", justifyContent: "center", mt: 2 }}>
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={handleSave}
                    sx={{
                      "&:disabled": {
                        opacity: 0.8,
                        backgroundColor: "#1976d2",
                      },
                    }}
                    disabled={saveLoading}
                  >
                    {saveLoading ? (
                      <CircularProgress size={24} sx={{ color: "#fff" }} />
                    ) : (
                      "Save"
                    )}
                  </Button>
                </Box>
              </>
            ) : null}
          </Grid>
        </Grid>
      </Box>
      <AddRoleModal
        open={open}
        onClose={() => {
          setOpen(false);
        }}
        initialData={selectedRow}
        isEditMode={isEditMode}
        fetchRoleData={() =>
          dispatch(fetchRoles({ ...tableRequest, pageNo: page + 1, pageSize }))
        }
      />
    </Box>
  );
};
