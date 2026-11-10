import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import EmployeeMasterService from "../services/EmployeeMasterService";

const service = new EmployeeMasterService();
export const saveEmployee = createAsyncThunk(
  "employee/saveEmployee",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.CreateUpdateUser(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const fetchUsers = createAsyncThunk(
  "employee/fetchUsers",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetUsers(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch users");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const employeeSlice = createSlice({
  name: "employee",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    createEmployeeError: null,
    createEmployeeSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createEmployeeError = null;
      state.createEmployeeSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.datasource = [];
        state.totalRecords = 0;
      })
      .addCase(saveEmployee.pending, (state, action) => {
        state.submitLoading = true;
        state.createEmployeeError = null;
      })
      .addCase(saveEmployee.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createEmployeeSuccess = action.payload;
      })
      .addCase(saveEmployee.rejected, (state, action) => {
        state.submitLoading = false;
        state.createEmployeeError = action.payload;
      });
  },
});
export const { reset } = employeeSlice.actions;
export default employeeSlice.reducer;
