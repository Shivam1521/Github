import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import DashboardService from "../services/DashboardService";

const service = new DashboardService();

/* ------------------ OVERVIEW ------------------ */

export const fetchDashboardOverview = createAsyncThunk(
  "dashboard/fetchOverview",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetDashboardOverview(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(
          res.responseMessage || "Failed to fetch overview"
        );
      }

      return res.responseData || {};
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);


export const fetchProductionAnalysis = createAsyncThunk(
  "dashboard/fetchProductionAnalysis",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetDashboardProductionAnalysis(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(
          res.responseMessage || "Failed to fetch production analysis"
        );
      }

      return res.responseData || {};
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

/* ------------------ DATA ------------------ */

export const fetchDashboardData = createAsyncThunk(
  "dashboard/fetchData",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetDashboardData(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch data");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

/* ------------------ FILTERS ------------------ */

export const fetchDashboardOverviewFilters = createAsyncThunk(
  "dashboard/fetchOverviewFilters",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetDashboardOverviewFilters(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(
          res.responseMessage || "Failed to fetch filters"
        );
      }

      return res.responseData || {};
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

/* ------------------ SLICE ------------------ */

const dashboardSlice = createSlice({
  name: "dashboard",

  initialState: {
    data: [],
    overviewData: {},
    filtersData: {},
    productionAnalysis: {},
    loading: false,
    error: null
  },

  reducers: {
    reset: (state) => {
      state.data = [];
      state.overviewData = {};
      state.filtersData = {};
      state.loading = false;
      state.error = null;
    }
  },

  extraReducers: (builder) => {

    builder

      /* -------- DASHBOARD DATA -------- */

      .addCase(fetchDashboardData.pending, (state) => {
        state.loading = true;
        state.error = null;

        state.data = []; // 👈 clear old data
      })

      .addCase(fetchDashboardData.fulfilled, (state, action) => {
        state.loading = false;

        state.data =
          action.payload && action.payload.length !== 0
            ? action.payload
            : []; // 👈 no data = empty
      })

      .addCase(fetchDashboardData.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;

        state.data = []; // 👈 error pe bhi clear
      })


      /* -------- OVERVIEW -------- */

      .addCase(fetchDashboardOverview.pending, (state) => {
        state.loading = true;
        state.error = null;

        state.overviewData = {}; // 👈 clear old data
      })

      .addCase(fetchDashboardOverview.fulfilled, (state, action) => {
        state.loading = false;

        state.overviewData =
          action.payload && Object.keys(action.payload).length !== 0
            ? action.payload
            : {}; // 👈 no data = empty object
      })

      .addCase(fetchDashboardOverview.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;

        state.overviewData = {}; // 👈 error pe bhi clear
      })


      /* -------- FILTERS -------- */

      .addCase(fetchDashboardOverviewFilters.pending, (state) => {
        state.loading = true;
        state.error = null;
      })

      .addCase(fetchDashboardOverviewFilters.fulfilled, (state, action) => {
        state.loading = false;
        state.filtersData = action.payload || {};
      })

      .addCase(fetchDashboardOverviewFilters.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      
      
.addCase(fetchProductionAnalysis.pending, (state) => {
  state.loading = true;
  state.error = null;
  state.productionAnalysis = {}; // clear old
})

.addCase(fetchProductionAnalysis.fulfilled, (state, action) => {
  state.loading = false;

  state.productionAnalysis =
    action.payload && Object.keys(action.payload).length !== 0
      ? action.payload
      : {};
})

.addCase(fetchProductionAnalysis.rejected, (state, action) => {
  state.loading = false;
  state.error = action.payload;
  state.productionAnalysis = {};
});

  },
});

export const { reset } = dashboardSlice.actions;

export default dashboardSlice.reducer;