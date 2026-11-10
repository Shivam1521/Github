import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import CommonService from "../services/CommonService";

const commonService = new CommonService();

export const fetchList = createAsyncThunk(
  "lists/fetchList",
  async (listName, { rejectWithValue }) => {
    try {
      const res = await commonService.GetList(listName);
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message);
    }
  }
);
export const fetchRoles = createAsyncThunk(
  "lists/fetchRoles",
  async (listName, { rejectWithValue }) => {
    try {
      const res = await commonService.GetRolesDDL(listName);
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message);
    }
  }
);
export const fetchUnits = createAsyncThunk(
  "lists/fetchUnits",
  async (listName, { rejectWithValue }) => {
    try {
      const res = await commonService.GetUnitsDDL();
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message);
    }
  }
);
export const fetchSubunits = createAsyncThunk(
  "lists/fetchSubunits",
  async (listName, { rejectWithValue }) => {
    try {
      const res = await commonService.GetPlantWarehouseDDL(listName);
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message);
    }
  }
);

const listSlice = createSlice({
  name: "lists",
  initialState: {
    roles: [],
    units: [],
    subUnits: [],
    whZones: [],
    listData: {},
    loading: false,
    error: null
  },
  reducers: {},
  extraReducers: (builder) => {
    // Fetch Generic List
    builder.addCase(fetchList.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchList.fulfilled, (state, action) => {
      state.loading = false;
      state.listData[action.meta.arg] = action.payload;
    });
    builder.addCase(fetchList.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload;
    });
    // Fetch Roles
    builder.addCase(fetchRoles.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchRoles.fulfilled, (state, action) => {
      state.loading = false;
      state.roles = action.payload;
    });
    builder.addCase(fetchRoles.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload;
    });
    // Fetch Units
    builder.addCase(fetchUnits.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchUnits.fulfilled, (state, action) => {
      state.loading = false;
      state.units = action.payload;
    });
    builder.addCase(fetchUnits.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload;
    });
    // Fetch SubUnits
    builder.addCase(fetchSubunits.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchSubunits.fulfilled, (state, action) => {
      state.loading = false;
      state.subUnits = action.payload;
    });
    builder.addCase(fetchSubunits.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload;
    });
  }
});

export default listSlice.reducer;
