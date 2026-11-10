import React, { useState, useEffect } from "react";
import {
  Modal,
  Fade,
  Backdrop,
  Box,
  Typography,
  Grid,
  TextField,
  Button,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Checkbox,
  Autocomplete,
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import { fetchList, fetchRoles, fetchUnits } from "../../store/commonSlice";
import { reset, saveEmployee } from "../../store/employeeMasterSlice";
import toast from "react-hot-toast";
import unitAccessHook from "../../components/UnitAccessHook";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 500,
  bgcolor: "background.paper",
  borderRadius: "8px",
  boxShadow: 24,
  p: 4,
};

const AddEmployeeModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchUserData,
}) => {
  const { isSuperAdmin, unitId } = unitAccessHook();
  const [formData, setFormData] = useState({
    mobileNo: "",
    email: "",
    userName: "",
    userCode: "",
    userType: "",
    roleId: "",
    password: "",
    whZoneId: null,
    unitId: unitId || "",
    lines: [],
  });

  const dispatch = useDispatch();

  const [errors, setErrors] = useState({});
  const [isPasswordReset, setIsPasswordReset] = useState(false);
  const {
    [`Line No&unitId=${formData.unitId}`]: LineNo,
    ["User Type"]: UserType,
    ["WHZone"]: WHZone,
  } = useSelector((state) => state.lists.listData);
  const { units, roles } = useSelector((state) => state.lists);
  const { submitLoading, createEmployeeError, createEmployeeSuccess } =
    useSelector((state) => state.employeeMaster);
  const userId = sessionStorage.getItem("userId");

  useEffect(() => {
    dispatch(fetchList("User Type"));
    dispatch(fetchList("WHZone"));
    dispatch(fetchUnits());
  }, []);

  useEffect(() => {
    if (formData.unitId) {
      dispatch(fetchList(`Line No&unitId=${formData.unitId}`));
    }
  }, [formData.unitId]);

  useEffect(() => {
    if (createEmployeeSuccess) {
      toast.success(createEmployeeSuccess);
      fetchUserData();
      onClose();
      dispatch(reset());
    }
    if (createEmployeeError) {
      toast.error(createEmployeeError);
      dispatch(reset());
    }
    return () => {};
  }, [createEmployeeSuccess, createEmployeeError]);

  useEffect(() => {
    if (isEditMode && initialData) {
      setFormData({
        mobileNo: initialData.mobileNo || "",
        email: initialData.email || "",
        userName: initialData.userName || "",
        userCode: initialData.userCode || "",
        userType: initialData.userType || "",
        roleId: initialData.roleId || "",
        whZoneId: initialData.whZoneId || null,
        unitId: initialData.unitId || "",
        lines: Array.isArray(initialData.lines)
          ? initialData.lines.map((line) => String(line.lineId))
          : [],
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

  // const validate = () => {
  //   const newErrors = {};
  //   if (!formData.mobileNo) newErrors.mobileNo = "Mobile number is required.";
  //   else if (!/^\d{10}$/.test(formData.mobileNo))
  //     newErrors.mobileNo = "Enter a valid 10-digit mobile number.";

  //   if (!formData.email) newErrors.email = "Email ID is required.";
  //   else if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(formData.email))
  //     newErrors.email = "Enter a valid email address.";
  //   if (!formData.userCode) newErrors.userCode = "Employee Code is required.";
  //   if (!formData.userName) newErrors.userName = "Employee Name is required.";
  //   if (!formData.userType) newErrors.userType = "Employee Type is required.";
  //   if (!formData.roleId) newErrors.roleId = "Role is required.";
  //   if (!formData.unitId) newErrors.unitId = "Unit is required.";
  //   // if (!formData.whZoneId) newErrors.whZoneId = "WHZone is required.";
  //   // if (!formData.lines || formData.lines.length === 0) {
  //   //   newErrors.lines = "Line No. is required.";
  //   // }

  //   if (!formData.password && !isEditMode)
  //     newErrors.password = "Password is required.";
  //   setErrors(newErrors);
  //   return Object.keys(newErrors).length === 0;
  // };

  const validate = () => {
  const newErrors = {};

  // Employee Code
  if (!formData.userCode?.trim()) {
    newErrors.userCode = "Employee Code is required.";
  } else if (!/^[a-zA-Z0-9_-]{1,30}$/.test(formData.userCode.trim())) {
    newErrors.userCode =
      "Employee Code can contain only letters, numbers, '_' and '-'.";
  }

  // Employee Name
  if (!formData.userName?.trim()) {
    newErrors.userName = "Employee Name is required.";
  } else if (!/^[a-zA-Z\s.'-]{2,50}$/.test(formData.userName.trim())) {
    newErrors.userName =
      "Employee Name can contain only letters, spaces, '.', ''' and '-'.";
  }

  // Mobile Number
  if (!formData.mobileNo?.trim()) {
    newErrors.mobileNo = "Mobile number is required.";
  } else if (!/^\d{10}$/.test(formData.mobileNo.trim())) {
    newErrors.mobileNo = "Enter a valid 10-digit mobile number.";
  }

  // Email
  if (!formData.email?.trim()) {
    newErrors.email = "Email ID is required.";
  } else if (
    !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(formData.email.trim())
  ) {
    newErrors.email = "Enter a valid email address.";
  }

  // Employee Type
  if (!formData.userType) {
    newErrors.userType = "Employee Type is required.";
  }

  // Role
  if (!formData.roleId) {
    newErrors.roleId = "Role is required.";
  }

  // Unit
  if (!formData.unitId) {
    newErrors.unitId = "Unit is required.";
  }

  // WH Zone - currently optional
  if (
    formData.whZoneId !== null &&
    formData.whZoneId !== "" &&
    !String(formData.whZoneId).trim()
  ) {
    newErrors.whZoneId = "Invalid WHZone.";
  }

  // Line No. - currently optional
  if (
    formData.lines &&
    (!Array.isArray(formData.lines) ||
      formData.lines.some((id) => !String(id).trim()))
  ) {
    newErrors.lines = "Invalid Line No.";
  }

  // Password
  if (!formData.password && !isEditMode) {
    newErrors.password = "Password is required.";
  } else if (formData.password) {
    if (formData.password.length < 8) {
      newErrors.password = "Password must be at least 8 characters.";
    } else if (formData.password.length > 50) {
      newErrors.password = "Password cannot exceed 50 characters.";
    }
  }

  setErrors(newErrors);

  return Object.keys(newErrors).length === 0;
};

  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };

  const generatePassword = () => {
    const charset =
      "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
    let password = "";
    for (let i = 0; i < 10; ++i) {
      password += charset.charAt(Math.floor(Math.random() * charset.length));
    }
    setFormData((prev) => ({ ...prev, password }));
    setIsPasswordReset(true);
    setErrors((prev) => ({ ...prev, password: "" }));
  };

  const handleSave = () => {
    if (validate()) {
      
      const payload = {
        userId: isEditMode ? initialData.userId : 0,
        userName: formData.userName,
        userCode: formData.userCode,
        userType: formData.userType,
        roleId: formData.roleId,
        email: formData.email,
        unitId: formData.unitId,
        lines: formData.lines.map((id) => ({ lineId: id })),
        whZoneId: formData.whZoneId,
        mobileNo: formData.mobileNo,
        isResetPassword: isEditMode ? initialData.isResetPassword : true,
        isActive: isEditMode ? initialData.isActive : true,
        createdBy: isEditMode ? initialData.createdBy : userId,
        ...(isPasswordReset && { password: formData.password }),
      };
      dispatch(saveEmployee(payload));
    }
  };

  const resetForm = () => {
    setFormData({
      userId: "",
      userName: "",
      userCode: "",
      userType: "",
      roleId: "",
      email: "",
      unitId: "",
      lines: [],
      whZoneId: null,
      mobileNo: "",
      isResetPassword: "",
      isActive: "",
      createdBy: "",
      password: "",
    });
    setErrors({});
    setIsPasswordReset(false);
  };

  const handleModalClose = () => {
    resetForm();
    onClose();
  };
  useEffect(() => {
    if (formData.userType) {
      dispatch(fetchRoles(formData.userType));
    }
  }, [formData.userType]);

  return (
    <Modal
      open={open}
      onClose={handleModalClose}
      closeAfterTransition
      slots={{ backdrop: Backdrop }}
      slotProps={{ backdrop: { timeout: 500 } }}
    >
      <Fade in={open}>
        <Box sx={style}>
          <Typography variant="h6" mb={2}>
            {isEditMode ? "Update Employee" : "Add New Employee"}
          </Typography>
          <Grid container spacing={2} className="d-flex flex-column">
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Employee Code"
                size="small"
                value={formData.userCode}
                onChange={handleChange("userCode")}
                error={!!errors.userCode}
                helperText={errors.userCode}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Employee Name"
                size="small"
                value={formData.userName}
                onChange={handleChange("userName")}
                error={!!errors.userName}
                helperText={errors.userName}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Mobile Number"
                size="small"
                value={formData.mobileNo}
                onChange={handleChange("mobileNo")}
                error={!!errors.mobileNo}
                helperText={errors.mobileNo}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Email ID"
                size="small"
                value={formData.email}
                onChange={handleChange("email")}
                error={!!errors.email}
                helperText={errors.email}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <FormControl fullWidth size="small" error={!!errors.userType}>
                <InputLabel>Employee Type</InputLabel>
                <Select
                  value={formData.userType}
                  label="Employee Type"
                  onChange={(e) => {
                    handleChange("userType")(e);
                    // dispatch(fetchRoles(e.target.value));
                  }}
                >
                  {UserType?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.userType && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.userType}
                  </Typography>
                )}
              </FormControl>
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <FormControl
                fullWidth
                size="small"
                error={!!errors.roleId}
                disabled={!formData.userType}
              >
                <InputLabel>Role</InputLabel>
                <Select
                  value={formData.roleId}
                  label="Role"
                  onChange={(e) => {
                    handleChange("roleId")(e);
                  }}
                >
                  {roles?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.roleId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.roleId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
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

            <Grid size={{ md: 6, xs: 6 }}>
              <FormControl fullWidth size="small" error={!!errors.whZoneId}>
                <Autocomplete
                  options={WHZone || []}
                  getOptionLabel={(option) => option.label || ""}
                  value={WHZone?.find(
                    (item) => String(item.value) === String(formData.whZoneId),
                  )}
                  onChange={(_, newValue) => {
                    handleChange("whZoneId")({
                      target: { value: newValue?.value || null },
                    });
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="WHZone"
                      size="small"
                      error={!!errors.whZoneId}
                      helperText={errors.whZoneId}
                    />
                  )}
                />
              </FormControl>
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.lines}>
                <Autocomplete
                  multiple
                  disableCloseOnSelect
                  options={LineNo || []}
                  getOptionLabel={(option) => option.label || ""}
                  value={
                    LineNo?.filter((item) =>
                      formData?.lines?.includes(item.value),
                    ) || []
                  }
                  onChange={(_, newValue) => {
                    handleChange("lines")({
                      target: { value: newValue.map((item) => item.value) },
                    });
                  }}
                  renderOption={(props, option, { selected }) => (
                    <li key={option.value} {...props}>
                      <Checkbox style={{ marginRight: 8 }} checked={selected} />
                      {option.label}
                    </li>
                  )}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Line No."
                      size="small"
                      error={!!errors.lines}
                      helperText={errors.lines}
                    />
                  )}
                />
              </FormControl>
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <Box display="flex" alignItems="center" gap={1}>
                <Button
                  fullWidth
                  variant="outlined"
                  onClick={generatePassword}
                  sx={{ textTransform: "capitalize", height: "40px" }}
                >
                  {isEditMode ? "Reset" : "Generate"} Password
                </Button>
                {formData.password && (
                  <Typography
                    width="100%"
                    variant="body1"
                    sx={{
                      px: 2,
                      py: 1,
                      ml: 1,
                      border: "1px solid",
                      borderColor: "rgba(0, 0, 0, 0.23)",
                      borderRadius: 1,
                      bgcolor: "rgba(255, 255, 255, 0.12)",
                      color: formData.password
                        ? "text.primary"
                        : "text.disabled",

                      minHeight: "40px",
                      display: "flex",
                      alignItems: "center",
                    }}
                  >
                    {formData.password || ""}
                  </Typography>
                )}
              </Box>
              {errors.password && (
                <Typography
                  variant="caption"
                  color="error"
                  textAlign="center"
                  sx={{ ml: 2, mt: -1 }}
                >
                  {errors.password}
                </Typography>
              )}
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <Box display="flex" justifyContent="space-between" gap={2}>
                <Button
                  fullWidth
                  variant="contained"
                  onClick={handleSave}
                  disabled={submitLoading}
                >
                  {submitLoading ? <CircularProgress size={20} /> : "Save"}
                </Button>
                <Button
                  fullWidth
                  variant="contained"
                  color="error"
                  onClick={handleModalClose}
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

export default AddEmployeeModal;
