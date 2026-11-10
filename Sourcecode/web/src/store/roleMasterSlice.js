import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import RoleMasterService from "../services/RoleMasterService";

const service = new RoleMasterService();
export const saveRole = createAsyncThunk(
  "role/saveRole",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.CreateUpdateRole(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);
export const savePermissions = createAsyncThunk(
  "role/savePermissions",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.AssignPermissions(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const fetchRoles = createAsyncThunk(
  "role/fetchRoles",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetRoles(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch roles");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);
export const fetchPages = createAsyncThunk(
  "role/fetchPages",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetPages(request);
      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch pages");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const roleSlice = createSlice({
  name: "role",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    createRoleError: null,
    createRoleSuccess: "",
    pageLoading: false,
    pages: [],
    saveLoading: false,
    permissionError: null,
    permissionSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createRoleError = null;
      state.createRoleSuccess = "";
    },
    resetPermissions: (state) => {
      state.permissionError = null;
      state.permissionSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchRoles.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchRoles.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchRoles.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.datasource = [];
        state.totalRecords = 0;
      })
      .addCase(fetchPages.pending, (state) => {
        state.pageLoading = true;
      })
      .addCase(fetchPages.fulfilled, (state, action) => {
        state.pageLoading = false;
        state.pages = action.payload;
      })
      .addCase(fetchPages.rejected, (state, action) => {
        state.pageLoading = false;
        state.error = action.payload;
      })
      .addCase(saveRole.pending, (state) => {
        state.submitLoading = true;
        state.createRoleError = null;
      })
      .addCase(saveRole.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createRoleSuccess = action.payload;
      })
      .addCase(saveRole.rejected, (state, action) => {
        state.submitLoading = false;
        state.createRoleError = action.payload;
      })
      .addCase(savePermissions.pending, (state) => {
        state.saveLoading = true;
        state.permissionError = null;
      })
      .addCase(savePermissions.fulfilled, (state, action) => {
        state.saveLoading = false;
        state.permissionSuccess = action.payload;
      })
      .addCase(savePermissions.rejected, (state, action) => {
        state.saveLoading = false;
        state.permissionError = action.payload;
      });
  },
});
export const { reset, resetPermissions } = roleSlice.actions;
export default roleSlice.reducer;
