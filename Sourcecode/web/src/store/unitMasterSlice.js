import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import UnitMasterService from "../services/UnitMasterService";

const service = new UnitMasterService();
export const saveUnit = createAsyncThunk(
  "unit/saveUnit",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.CreateUpdateUnit(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

export const fetchUnits = createAsyncThunk(
  "unit/fetchUnits",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetUnits(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch users");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

const unitSlice = createSlice({
  name: "unit",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    createUnitError: null,
    createUnitSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createUnitError = null;
      state.createUnitSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUnits.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUnits.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchUnits.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(saveUnit.pending, (state, action) => {
        state.submitLoading = true;
        state.createUnitError = null;
      })
      .addCase(saveUnit.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createUnitSuccess = action.payload;
      })
      .addCase(saveUnit.rejected, (state, action) => {
        state.submitLoading = false;
        state.createUnitError = action.payload;
      });
  },
});
export const { reset } = unitSlice.actions;
export default unitSlice.reducer;
