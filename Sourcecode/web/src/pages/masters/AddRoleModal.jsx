import React, { useEffect, useState } from "react";
import {
  Backdrop,
  Box,
  Modal,
  Fade,
  Button,
  Typography,
  CircularProgress,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  TextField,
  Autocomplete,
} from "@mui/material";
import toast from "react-hot-toast";
import { useDispatch, useSelector } from "react-redux";
import { fetchList, fetchUnits } from "../../store/commonSlice";
import { reset, saveRole } from "../../store/roleMasterSlice";
import unitAccessHook from "../../components/UnitAccessHook";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  bgcolor: "background.paper",
  borderRadius: "8px",
  boxShadow: 24,
  p: 4,
};

const AddRoleModal = ({
  open,
  onClose,
  isEditMode,
  initialData,
  fetchRoleData,
}) => {
  const dispatch = useDispatch();
  const { isSuperAdmin, unitId } = unitAccessHook();
  const [errors, setErrors] = useState({});
  const { units } = useSelector((state) => state.lists);
  const [formData, setFormData] = useState({
    roleName: "",
    roleDesc: "",
    unitId: "",
    userTypeId: "",
  });

  const { ["User Type"]: UserType } = useSelector(
    (state) => state.lists.listData,
  );
  const { submitLoading, createRoleError, createRoleSuccess } = useSelector(
    (state) => state.roleMaster,
  );

  useEffect(() => {
    dispatch(fetchList("User Type"));
    dispatch(fetchUnits());
  }, []);

  useEffect(() => {
    if (createRoleSuccess) {
      toast.success(createRoleSuccess);
      fetchRoleData();
      onClose();
      dispatch(reset());
    }
    if (createRoleError) {
      toast.error(createRoleError);
      dispatch(reset());
    }
    return () => {};
  }, [createRoleSuccess, createRoleError]);
  const handleChange = (field) => (e) => {
    setFormData({ ...formData, [field]: e.target.value });
    setErrors({ ...errors, [field]: "" });
  };

  // const validate = () => {
  //   const newErrors = {};
  //   if (!formData.roleName) newErrors.roleName = "Role Name is required.";
  //   if (!formData.roleDesc)
  //     newErrors.roleDesc = "Role Description is required.";
  //   if (!formData.userTypeId) newErrors.userTypeId = "User Type is required.";
  //   //if (!formData.unitId) newErrors.unitId = "Unit is required.";

  //   setErrors(newErrors);
  //   return Object.keys(newErrors).length === 0;
  // };
  
  const validate = () => {
  const newErrors = {};

  // Role Name
  if (!formData.roleName?.trim()) {
    newErrors.roleName = "Role Name is required.";
  } else if (
    !/^[a-zA-Z0-9\s.'()/_&-]{2,50}$/.test(formData.roleName.trim())
  ) {
    newErrors.roleName = "Role Name contains invalid characters.";
  }

  // Role Description
  if (!formData.roleDesc?.trim()) {
    newErrors.roleDesc = "Role Description is required.";
  } else if (
    !/^[a-zA-Z0-9\s.,'()/_&:#@+-]{2,200}$/.test(
      formData.roleDesc.trim()
    )
  ) {
    newErrors.roleDesc =
      "Role Description contains invalid characters.";
  }

  // User Type
  if (!formData.userTypeId) {
    newErrors.userTypeId = "User Type is required.";
  }

  // Unit is optional currently
  // if (!formData.unitId) {
  //   newErrors.unitId = "Unit is required.";
  // }

  setErrors(newErrors);

  return Object.keys(newErrors).length === 0;
};
  const handleSave = () => {
    if (validate()) {
      const payload = {
        roleId: isEditMode ? initialData.roleId : 0,
        roleName: formData.roleName,
        userTypeId: String(formData.userTypeId),
        unitId:
         formData.unitId === "" ||
         formData.unitId === 0 ||
         formData.unitId == null
         ? null
         : Number(formData.unitId),
        roleDesc: formData.roleDesc,
        isActive: isEditMode ? initialData.isActive : true,
        createdBy: isEditMode
          ? initialData.createdBy
          : sessionStorage.getItem("userId"),
      };
      dispatch(saveRole(payload));
    }
  };

  const resetForm = () => {
    setFormData({
      roleDesc: "",
      roleName: "",
      unitId: "",
      userTypeId: "",
    });
    setErrors({});
  };

  useEffect(() => {
    if (isEditMode && initialData) {
      setFormData({
        roleName: initialData.roleName || "",
        roleDesc: initialData.roleDesc || "",
        userTypeId: initialData.userTypeId || "",
        unitId: String(initialData.unitId || ""),
      });
    } else if (open && !isEditMode) {
      resetForm();
    }
  }, [initialData, isEditMode, open]);

  useEffect(() => {
    if (open && !isEditMode) {
      setFormData((prev) => ({
        ...prev,
        unitId: String(unitId),
      }));
    }
  }, [open, isEditMode]);

  const handleModalClose = () => {
    resetForm();
    setErrors({});
    onClose();
  };

  return (
    <Modal
      aria-labelledby="transition-modal-title"
      aria-describedby="transition-modal-description"
      open={open}
      onClose={handleModalClose}
      closeAfterTransition
      slots={{ backdrop: Backdrop }}
      slotProps={{
        backdrop: {
          timeout: 500,
        },
      }}
    >
      <Fade in={open}>
        <Box sx={style}>
          <Typography variant="h6" mb={2}>
            {isEditMode ? "Update Role" : "Add New Role"}
          </Typography>

          <Grid container spacing={2}>
            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.unitId}>
                <Autocomplete
                  options={units || []}
                  disabled={!isSuperAdmin}
                  getOptionLabel={(option) => option.label || ""}
                  value={
                    units?.find(
                      (item) => item.value === String(formData.unitId),
                    ) || null
                  }
                  onChange={(_, newValue) => {
                    handleChange("unitId")({
                      target: { value: newValue?.value || "" },
                    });
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Unit"
                      size="small"
                      error={!!errors.unitId}
                      helperText={errors.unitId}
                    />
                  )}
                />
              </FormControl>
            </Grid>
            <Grid size={{ md: 12, xs: 12 }}>
              <TextField
                fullWidth
                required
                label="Role Name"
                size="small"
                value={formData.roleName}
                onChange={handleChange("roleName")}
                error={!!errors.roleName}
                helperText={errors.roleName}
              />
            </Grid>
            <Grid size={{ md: 12, xs: 12 }}>
              <TextField
                fullWidth
                required
                label="Role Description"
                size="small"
                value={formData.roleDesc}
                onChange={handleChange("roleDesc")}
                error={!!errors.roleDesc}
                helperText={errors.roleDesc}
              />
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl
                fullWidth
                required
                size="small"
                error={!!errors.userTypeId}
              >
                <InputLabel>Employee Type</InputLabel>
                <Select
                  value={formData.userTypeId}
                  label="Employee Type"
                  onChange={handleChange("userTypeId")}
                >
                  {UserType?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.userTypeId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.userTypeId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <Box display="flex" gap={2}>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleSave}
                  fullWidth
                  disabled={submitLoading}
                  sx={{
                    "&:disabled": {
                      opacity: 0.8,
                      backgroundColor: "#1976d2",
                    },
                  }}
                >
                  {submitLoading ? (
                    <CircularProgress size={24} sx={{ color: "#fff" }} />
                  ) : (
                    "Save"
                  )}
                </Button>
                <Button
                  variant="contained"
                  color="error"
                  onClick={onClose}
                  fullWidth
                >
                  Close
                </Button>
              </Box>
            </Grid>
          </Grid>
        </Box>
      </Fade>
    </Modal>
  );
};

export default AddRoleModal;
