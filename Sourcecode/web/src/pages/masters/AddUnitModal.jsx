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
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import { fetchList } from "../../store/commonSlice";
import { reset, saveUnit } from "../../store/unitMasterSlice";
import toast from "react-hot-toast";

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

const AddUnitModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchUnitData,
}) => {
  const [formData, setFormData] = useState({
    unitId: "",
    unitName: "",
    unitCode: "",
    email: "",
    locationId: "",
    mobileNumber: "",
    address: "",
    city: "",
    state: "",
    country: "",
    pincode: "",
  });
  const dispatch = useDispatch();
  const [errors, setErrors] = useState({});
  const { Location } = useSelector((state) => state.lists.listData);
  const { submitLoading, createUnitError, createUnitSuccess } = useSelector(
    (state) => state.unitMaster
  );

  const userId = sessionStorage.getItem("userId");

  useEffect(() => {
    dispatch(fetchList("Location"));
  }, []);

  useEffect(() => {
    if (createUnitSuccess) {
      toast.success(createUnitSuccess);
      fetchUnitData();
      onClose();
      dispatch(reset());
    }
    if (createUnitError) {
      toast.error(createUnitError);
      dispatch(reset());
    }
    return () => {};
  }, [createUnitSuccess, createUnitError]);

  useEffect(() => {
    if (isEditMode && initialData) {
      setFormData({
        mobileNumber: initialData.mobileNumber || "",
        email: initialData.email || "",
        unitName: initialData.unitName || "",
        unitCode: initialData.unitCode || "",
        address: initialData.address || "",
        city: initialData.city || "",
        state: initialData.state || "",
        country: initialData.country || "",
        pincode: initialData.pincode || "",
      });
    } else if (open && !isEditMode) {
      resetForm();
    }
  }, [initialData, isEditMode, open]);

  const validate = () => {
    const newErrors = {};

    if (!formData.unitName) newErrors.unitName = "Unit Name is required.";
    if (!formData.unitCode) newErrors.unitCode = "Unit Code is required.";

    if (!formData.email) newErrors.email = "Email is required.";
    else if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(formData.email))
      newErrors.email = "Enter a valid email address.";

    if (!formData.mobileNumber)
      newErrors.mobileNumber = "Mobile Number is required.";
    else if (!/^\d{10}$/.test(formData.mobileNumber))
      newErrors.mobileNumber = "Enter a valid 10-digit mobile number.";

    if (!formData.locationId) newErrors.locationId = "Location is required.";
    if (!formData.city) newErrors.city = "City is required.";
    if (!formData.state) newErrors.state = "State is required.";
    if (!formData.country) newErrors.country = "Country is required.";
    if (!formData.pincode) newErrors.pincode = "Pincode is required.";
    else if (!/^\d{6}$/.test(formData.pincode))
      newErrors.pincode = "Enter a valid 6-digit pincode.";

    if (!formData.address) newErrors.address = "Address is required.";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };
  const handleSave = () => {
    if (validate()) {
      const payload = {
        unitId: isEditMode ? initialData.unitId : 0,
        unitName: formData.unitName,
        unitCode: formData.unitCode,
        email: formData.email,
        locationId: formData.locationId,
        mobileNumber: formData.mobileNumber,
        address: formData.address,
        city: formData.city,
        state: formData.state,
        country: formData.country,
        pincode: formData.pincode,
        userId: userId,
      };
      dispatch(saveUnit(payload));
    }
  };
  const resetForm = () => {
    setFormData({
      unitId: "",
      unitName: "",
      unitCode: "",
      email: "",
      locationId: "",
      mobileNumber: "",
      address: "",
      city: "",
      state: "",
      country: "",
      pincode: "",
      userId: "",
    });
    setErrors({});
  };

  const handleModalClose = () => {
    resetForm();
    onClose();
  };

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
            {isEditMode ? "Update Unit" : "Add New Unit"}
          </Typography>

          <Grid container spacing={2} className="d-flex flex-column">
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Unit Code"
                size="small"
                value={formData.unitCode}
                onChange={handleChange("unitCode")}
                error={!!errors.unitCode}
                helperText={errors.unitCode}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Unit Name"
                size="small"
                value={formData.unitName}
                onChange={handleChange("unitName")}
                error={!!errors.unitName}
                helperText={errors.unitName}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Email"
                size="small"
                value={formData.email}
                onChange={handleChange("email")}
                error={!!errors.email}
                helperText={errors.email}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Mobile Number"
                size="small"
                value={formData.mobileNumber}
                onChange={handleChange("mobileNumber")}
                error={!!errors.mobileNumber}
                helperText={errors.mobileNumber}
              />
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.locationId}>
                <InputLabel>Location</InputLabel>
                <Select
                  value={formData.locationId}
                  label="Location"
                  onChange={(e) => {
                    handleChange("locationId")(e);
                  }}
                >
                  {Location?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.locationId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.locationId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="City"
                size="small"
                value={formData.city}
                onChange={handleChange("city")}
                error={!!errors.city}
                helperText={errors.city}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="State"
                size="small"
                value={formData.state}
                onChange={handleChange("state")}
                error={!!errors.state}
                helperText={errors.state}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Country"
                size="small"
                value={formData.country}
                onChange={handleChange("country")}
                error={!!errors.country}
                helperText={errors.country}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Pincode"
                size="small"
                value={formData.pincode}
                onChange={handleChange("pincode")}
                error={!!errors.pincode}
                helperText={errors.pincode}
              />
            </Grid>

            <Grid size={{ md: 12, xs: 12 }}>
              <TextField
                fullWidth
                label="Address"
                size="small"
                minRows={2}
                multiline
                value={formData.address}
                onChange={handleChange("address")}
                error={!!errors.address}
                helperText={errors.address}
              />
            </Grid>
          </Grid>

          <Grid size={{ md: 12 }} sx={{ mt: 2 }}>
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
        </Box>
      </Fade>
    </Modal>
  );
};

export default AddUnitModal;
