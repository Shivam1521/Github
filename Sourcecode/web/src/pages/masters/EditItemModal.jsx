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
  FormControlLabel,
  FormGroup,
  Autocomplete,
  Slider,
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import { fetchList } from "../../store/commonSlice";
import toast from "react-hot-toast";
import { reset, updateItem } from "../../store/itemMasterSlice";
import unitAccessHook from "../../components/UnitAccessHook";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 700,
  bgcolor: "background.paper",
  borderRadius: "8px",
  boxShadow: 24,
  p: 4,
  maxHeight: "100%",
  overflowY: "auto",
};

const EditItemModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchUserData,
}) => {
  const { unitId } = unitAccessHook();
  const [formData, setFormData] = useState({
    itemCode: "",
    description: "",
    brand: "",
    mfgLocation: "",
    class: "0",
    masterCartonPackingSize: "",
    outerBoxPackingSize: "",
    outerPerCarton: null,
    labelPerLot: "",
    pic1LineSupervisor: "",
    pic2Engineer: "",
    lotQty: null,
    totalCards: null,
    totalRooms: null,
    cartonPerOuterBoxInLot: null,
    whZoneID: null,
    lines: [],
    placeSerial: "",
    location: "",
    productionZone: "",
    priority: "",
    samriddhi: "",
    smartSaver: "",
    id: "",
    outerBoxWeightRange: [],
  });

  const dispatch = useDispatch();
  const [errors, setErrors] = useState({});
  const { WHZone, [`Line No&unitId=${unitId}`]: LineNo } = useSelector(
    (state) => state.lists.listData,
  );

  const { submitLoading, updateItemError, updateItemSuccess } = useSelector(
    (state) => state.itemMaster,
  );

  useEffect(() => {
    dispatch(fetchList("WHZone"));
  }, []);
  useEffect(() => {
    if (unitId) {
      dispatch(fetchList(`Line No&unitId=${unitId}`));
    }
  }, [unitId]);

  useEffect(() => {
    if (updateItemSuccess) {
      toast.success(updateItemSuccess);
      fetchUserData();
      onClose();
      dispatch(reset());
    }
    if (updateItemError) {
      toast.error(updateItemError);
      dispatch(reset());
    }
    return () => {};
  }, [updateItemSuccess, updateItemError]);

  useEffect(() => {
    if (initialData) {
      setFormData({
        itemCode: initialData.itemCode || "",
        description: initialData.description || "",
        brand: initialData.brand || "",
        mfgLocation: initialData.mfgLocation || "",
        class: initialData.class || "0",
        masterCartonPackingSize: initialData.masterCartonPackingSize || "",
        outerBoxPackingSize: initialData.outerBoxPackingSize || "",
        outerPerCarton: initialData.outerPerCarton ?? null,
        labelPerLot: initialData.labelPerLot || "",
        pic1LineSupervisor: initialData.pic1LineSupervisor || "",
        pic2Engineer: initialData.pic2Engineer || "",
        lotQty: initialData.lotQty ?? null,
        totalCards: initialData.totalCards ?? null,
        totalRooms: initialData.totalRooms ?? null,
        outerBoxWeightRange:
          Array.isArray(initialData.outerBoxWeightRange) &&
          initialData.outerBoxWeightRange.length === 2
            ? initialData.outerBoxWeightRange
            : [0, 0],

        cartonPerOuterBoxInLot: initialData.cartonPerOuterBoxInLot ?? null,
        whZoneID: initialData.whZoneID || null,
        lines: Array.isArray(initialData.lines)
          ? initialData.lines.map((line) => String(line.lineId))
          : [],
        placeSerial: initialData.placeSerial || "",
        location: initialData.location || "",
        productionZone: initialData.productionZone || "",
        priority: initialData.priority || "",
        samriddhi: initialData.samriddhi ?? false,
        smartSaver: initialData.smartSaver ?? false,
        id: initialData.id || "",
      });
    }
  }, [initialData]);

const validate = () => {
  const newErrors = {};

  // ==========================================
  // String fields - Malicious Input Validation
  // ==========================================

  const stringFields = [
    "pic1LineSupervisor",
    "pic2Engineer",
    "placeSerial",
    "productionZone",
    "location",
    "priority",
  ];

  const maliciousPattern =
    /<script\b[^>]*>[\s\S]*?<\/script\s*>|javascript\s*:|on\w+\s*=/i;

  stringFields.forEach((field) => {
    const value = formData[field];

    if (
      value &&
      typeof value === "string" &&
      maliciousPattern.test(value)
    ) {
      newErrors[field] =
        "Special characters or script content are not allowed.";
    }
  });

  // ==========================================
  // Integer fields - Only Numbers
  // ==========================================

  const integerFields = [
    "masterCartonPackingSize",
    "outerBoxPackingSize",
    "lotQty",
    "totalCards",
    "totalRooms",
    "cartonPerOuterBoxInLot",
  ];

  integerFields.forEach((field) => {
    const value = formData[field];

    if (
      value !== null &&
      value !== "" &&
      !/^\d+$/.test(String(value))
    ) {
      newErrors[field] = "Only numbers are allowed.";
    }
  });

  // ==========================================
  // Existing Outer Box Weight Validation
  // ==========================================

  const [min, max] = formData.outerBoxWeightRange;

  newErrors.outerBoxWeightRange = [];

  if (min > max) {
    newErrors.outerBoxWeightRange[0] =
      "Min weight cannot exceed Max weight";
  } else if (max < min) {
    newErrors.outerBoxWeightRange[1] =
      "Max weight cannot be less than Min weight";
  } else if (
    min === 0 &&
    formData.outerBoxWeightRange[0] !== 0
  ) {
    newErrors.outerBoxWeightRange[0] =
      "Min weight must be at least 0.1 kg";
  } else if (max > 1) {
    newErrors.outerBoxWeightRange[1] =
      "Max weight cannot exceed 1 kg";
  }

  // Remove weight error if there is no error
  if (
    !newErrors.outerBoxWeightRange[0] &&
    !newErrors.outerBoxWeightRange[1]
  ) {
    delete newErrors.outerBoxWeightRange;
  }

  // ==========================================
  // Set Errors
  // ==========================================

  setErrors(newErrors);

  return Object.keys(newErrors).length === 0;
};

  const handleChange = (field) => (event) => {
    const value =
      field === "whZoneID" && event.target.value === ""
        ? null
        : event.target.value;
    setFormData({ ...formData, [field]: value });
    setErrors({ ...errors, [field]: "" });
  };

  const handleSave = () => {
    if (validate()) {
      const payload = {
        id: formData.id || 0,
        outerPerCarton: formData.outerPerCarton,
        labelPerLot: formData.labelPerLot,
        pic1LineSupervisor: formData.pic1LineSupervisor,
        pic2Engineer: formData.pic2Engineer,
        lotQty: formData.lotQty,
        totalCards: formData.totalCards,
        totalRooms: formData.totalRooms,
        cartonPerOuterBoxInLot: formData.cartonPerOuterBoxInLot,
        whZoneID: formData.whZoneID,
        lines: formData.lines.map((id) => ({ lineId: id })),
        placeSerial: formData.placeSerial,
        location: formData.location,
        productionZone: formData.productionZone,
        priority: formData.priority,
        samriddhi: formData.samriddhi,
        smartSaver: formData.smartSaver,
        outerBoxWeightRange: formData.outerBoxWeightRange,
        masterCartonPackingSize: formData.masterCartonPackingSize,
        outerBoxPackingSize: formData.outerBoxPackingSize,
      };

      dispatch(updateItem(payload));
    }
  };

  const resetForm = () => {
    setFormData({
      outerPerCarton: null,
      labelPerLot: "",
      pic1LineSupervisor: "",
      pic2Engineer: "",
      lotQty: null,
      totalCards: null,
      totalRooms: null,
      cartonPerOuterBoxInLot: null,
      whZoneID: null,
      lines: [],
      placeSerial: "",
      location: "",
      productionZone: "",
      priority: "",
      outerBoxWeightRange: [],
         masterCartonPackingSize: "",
        outerBoxPackingSize: "",
      samriddhi: false,
      smartSaver: false,
      id: "",
    });
    setErrors({});
  };

  const handleModalClose = () => {
    resetForm();
    onClose();
  };

  const handleWeightChange = (index, newValue) => {
    if (newValue === "") {
      setFormData((prev) => {
        const updated = [...(prev.outerBoxWeightRange || [0, 0])];
        updated[index] = "";
        return { ...prev, outerBoxWeightRange: updated };
      });
      return;
    }

    const decimalRegex = /^\d*\.?\d{0,5}$/;

    if (decimalRegex.test(newValue)) {
      let value = parseFloat(newValue);

      setFormData((prev) => {
        const updated = [...(prev.outerBoxWeightRange || [0, 0])];
        updated[index] = value;
        return { ...prev, outerBoxWeightRange: updated };
      });
    }
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
            Update Item
          </Typography>
          <Grid
            container
            spacing={2}
            className="d-flex flex-column align-items-flex-start"
          >
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Item Code"
                size="small"
                value={formData.itemCode}
                disabled
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Description"
                size="small"
                disabled
                value={formData.description}
              />
            </Grid>

            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Brand"
                size="small"
                disabled
                value={formData.brand}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Mfg Location"
                size="small"
                disabled
                value={formData.mfgLocation}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Class"
                size="small"
                disabled
                value={formData.class}
              />
            </Grid>

            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Master Carton Packing"
                size="small"
                //disabled
                value={formData.masterCartonPackingSize}
                 onChange={handleChange("masterCartonPackingSize")}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Outer Box Packing"
                size="small"
                //disabled
                value={formData.outerBoxPackingSize}
                onChange={handleChange("outerBoxPackingSize")}
              />
            </Grid>
            {/* <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Outer/Carton"
                size="small"
                value={formData.outerPerCarton}
                onChange={handleChange("outerPerCarton")}
                // error={!!errors.outerPerCarton}
                // helperText={errors.outerPerCarton}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Label/Lot"
                size="small"
                value={formData.labelPerLot}
                onChange={handleChange("labelPerLot")}
                // error={!!errors.labelPerLot}
                // helperText={errors.labelPerLot}
              />
            </Grid> */}
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Pic 1 ( Line Supervisor)"
                size="small"
                value={formData.pic1LineSupervisor}
                onChange={handleChange("pic1LineSupervisor")}
                error={!!errors.pic1LineSupervisor}
                helperText={errors.pic1LineSupervisor}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="PIC 2 ( Engineer)"
                size="small"
                value={formData.pic2Engineer}
                onChange={handleChange("pic2Engineer")}
                error={!!errors.pic2Engineer}
                helperText={errors.pic2Engineer}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Lot Qty"
                size="small"
                value={formData.lotQty}
                onChange={handleChange("lotQty")}
                error={!!errors.lotQty}
                helperText={errors.lotQty}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Total Cards"
                size="small"
                value={formData.totalCards}
                onChange={handleChange("totalCards")}
                error={!!errors.totalCards}
                helperText={errors.totalCards}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Total Room"
                size="small"
                value={formData.totalRooms}
                onChange={handleChange("totalRooms")}
                error={!!errors.totalRooms}
                helperText={errors.totalRooms}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Carton/Outer Box In Lot"
                size="small"
                value={formData.cartonPerOuterBoxInLot}
                onChange={handleChange("cartonPerOuterBoxInLot")}
                error={!!errors.cartonPerOuterBoxInLot}
                helperText={errors.cartonPerOuterBoxInLot}
              />
            </Grid>

            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Place Serial"
                size="small"
                value={formData.placeSerial}
                onChange={handleChange("placeSerial")}
                error={!!errors.placeSerial}
                helperText={errors.placeSerial}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Production Zone"
                size="small"
                value={formData.productionZone}
                onChange={handleChange("productionZone")}
                error={!!errors.productionZone}
                helperText={errors.productionZone}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <FormControl fullWidth size="small" error={!!errors.whZoneID}>
                <Autocomplete
                  options={WHZone || []}
                  getOptionLabel={(option) => option.label || ""}
                  value={WHZone?.find(
                    (item) => String(item.value) === String(formData.whZoneID),
                  )}
                  onChange={(_, newValue) => {
                    handleChange("whZoneID")({
                      target: { value: newValue?.value || "" },
                    });
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="WH Zone"
                      size="small"
                      error={!!errors.whZoneID}
                      helperText={errors.whZoneID}
                    />
                  )}
                />
              </FormControl>
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Location"
                size="small"
                value={formData.location}
                onChange={handleChange("location")}
                error={!!errors.location}
                helperText={errors.location}
              />
            </Grid>
            <Grid size={{ md: 4, xs: 4 }}>
              <TextField
                fullWidth
                label="Priority"
                size="small"
                value={formData.priority}
                onChange={handleChange("priority")}
                error={!!errors.priority}
                helperText={errors.priority}
              />
            </Grid>

            {/* <Box sx={{ display: "flex", flexDirection: "row" }}> */}

            <Grid size={{ md: 6, xs: 12 }}>
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
                  renderOption={(props, option, { selected }) => {
                    const { key, ...otherProps } = props;
                    return (
                      <li key={option.value} {...otherProps}>
                        <Checkbox
                          style={{ marginRight: 8 }}
                          checked={selected}
                        />
                        {option.label}
                      </li>
                    );
                  }}
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
            <Grid size={{ md: 3, xs: 4 }}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.samriddhi}
                    onChange={(e) =>
                      setFormData((prev) => ({
                        ...prev,
                        samriddhi: e.target.checked,
                      }))
                    }
                  />
                }
                label="Samriddhi"
              />
            </Grid>
            <Grid size={{ md: 3, xs: 4 }}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.smartSaver}
                    onChange={(e) =>
                      setFormData((prev) => ({
                        ...prev,
                        smartSaver: e.target.checked,
                      }))
                    }
                  />
                }
                label="Smart Saver"
              />{" "}
            </Grid>
            <Grid size={{ md: 6, xs: 12 }}>
              <Typography
                gutterBottom
                sx={{
                  fontWeight: 500,
                  color: "#374151",
                  mb: 3.5,
                }}
              >
                Outer Box Weight (Min & Max)
              </Typography>

              <Slider
                value={formData.outerBoxWeightRange || [0, 0]}
                onChange={handleChange("outerBoxWeightRange")}
                valueLabelDisplay="on"
                min={0}
                max={1}
                step={0.01}
                disableSwap
                getAriaLabel={() => "Outer Box Weight"}
                getAriaValueText={(value) => `${value.toFixed(2)} kg`}
                sx={{
                  color: "rgb(79, 78, 78)",
                  height: 8,
                  ml: 1,
                  borderRadius: 2,
                  "& .MuiSlider-thumb": {
                    height: 20,
                    width: 20,
                    backgroundColor: "#fff",
                    border: "2px solid currentColor",
                    boxShadow: "0 2px 6px rgba(0,0,0,0.2)",
                    "&:hover": {
                      boxShadow: "0 0 0 8px rgba(25, 118, 210, 0.16)",
                    },
                  },
                  "& .MuiSlider-track": {
                    border: "none",
                  },
                  "& .MuiSlider-rail": {
                    opacity: 0.3,
                    backgroundColor: "#b0bec5",
                  },
                  "& .MuiSlider-valueLabel": {
                    // backgroundColor: "#1976d2",
                    color: "#fff",
                    borderRadius: "8px",
                    fontSize: "0.75rem",
                    fontWeight: 500,
                  },
                }}
              />
            </Grid>
            <Grid size={{ md: 6, sx: 12 }}>
              {" "}
              <Box
                display="flex"
                justifyContent="space-between"
                gap={1}
                mt={5}
                ml={2}
              >
                <TextField
                  label="Min (kg)"
                  size="small"
                  type="number"
                  inputProps={{ min: 0, max: 1, step: 0.01 }}
                  value={formData?.outerBoxWeightRange?.[0] ?? 0}
                  onChange={(e) => handleWeightChange(0, e.target.value)}
                  sx={{ flex: 1 }}
                  error={!!errors?.outerBoxWeightRange?.[0]}
                  helperText={errors?.outerBoxWeightRange?.[0]}
                />
                <TextField
                  label="Max (kg)"
                  size="small"
                  inputProps={{ min: 0, max: 1, step: 0.01 }}
                  value={formData?.outerBoxWeightRange?.[1] ?? 0}
                  onChange={(e) => handleWeightChange(1, e.target.value)}
                  sx={{ flex: 1 }}
                  type="number"
                  error={!!errors?.outerBoxWeightRange?.[1]}
                  helperText={errors?.outerBoxWeightRange?.[1]}
                />
              </Box>
            </Grid>
            {/* </Box> */}

            <Grid size={{ md: 12, xs: 12 }}>
              <Box display="flex" justifyContent="center" gap={2}>
                <Button
                  variant="contained"
                  onClick={handleSave}
                  disabled={submitLoading}
                >
                  {submitLoading ? <CircularProgress size={20} /> : "Save"}
                </Button>
                <Button
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

export default EditItemModal;
