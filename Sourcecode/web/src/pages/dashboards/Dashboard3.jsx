import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchDashboardOverview,
  fetchDashboardOverviewFilters,
  fetchProductionAnalysis
} from "../../store/dashboardSlice";

import { Overview } from "./Overview";
import { ProductionAnalysis } from "./ProductionAnalysis";
import "./Dashboard.scss";

/* ---------- Icons ---------- */

const TuneIcon = () => (
  <svg width="11" height="11" viewBox="0 0 24 24" fill="currentColor">
    <path d="M3 17v2h6v-2H3zM3 5v2h10V5H3zm10 16v-2h8v-2h-8v-2h-2v6h2z"/>
  </svg>
);

const FilterIcon = () => (
  <svg width="11" height="11" viewBox="0 0 24 24" fill="currentColor">
    <path d="M4 5h16L14 13v6h-4v-6L4 5z"/>
  </svg>
);

/* ---------- Filter Section ---------- */
const FilterSection = ({
  title,
  items,
  selected,
  onSelect,
  showSearch
}) => {
  const [search, setSearch] = React.useState("");

  const filteredItems = items.filter(item =>
    item.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="filter-section">
      <div className="filter-header">
        <span className="filter-title">{title}</span>

        <div className="filter-actions">
          <button className="icon-btn"><TuneIcon/></button>
          <button className="icon-btn"><FilterIcon/></button>
        </div>
      </div>

      {showSearch && (
        <input
          type="text"
          placeholder={`Search ${title}`}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="filter-search"
        />
      )}

      <div className="filter-chips">
        {filteredItems.map(item => (
          <button
            key={item}
            onClick={() => onSelect(item)}
            className={`filter-chip ${
              selected === item ? "active" : ""
            }`}
          >
            {item}
          </button>
        ))}
      </div>
    </div>
  );
};

// const FilterSection = ({ title, items, selected, onSelect }) => (
//   <div className="filter-section">
//     <div className="filter-header">
//       <span className="filter-title">{title}</span>
//       <div className="filter-actions">
//         <button className="icon-btn"><TuneIcon/></button>
//         <button className="icon-btn"><FilterIcon/></button>
//       </div>
//     </div>

//     <div className="filter-chips">
//       {items.map(item => (
//         <button
//           key={item}
//           onClick={() => onSelect(item)}
//           className={`filter-chip ${selected === item ? "active" : ""}`}
//         >
//           {item}
//         </button>
//       ))}
//     </div>
//   </div>
// );

/* ---------- Sidebar ---------- */

const LeftSidebar = ({
  tab,
  onFilterChange,
  filtersData,
  filters,
  setFilters
}) => {

  const today = new Date().toISOString().split("T")[0];

  const getFYDates = (fy) => {
    if (fy === "All") return null;
    const year = fy.replace("FY ", "");
    const startYear = parseInt(year.split("-")[0]);
    const endYear = startYear + 1;

    return {
      from: `${startYear}-04-01`,
      to: `${endYear}-03-31`
    };
  };

  // const [filters, setFilters] = useState({
  //   fromDate: "2025-04-01",
  //   toDate: today,
  //   unitId: null,
  //   locationType: null,
  //   bu: null,
  //   section: null,
  //   brand: null,
  //   category: null
  // });
  

  const [fy, setFy] = useState("All");
  const [quarter, setQuarter] = useState("All");

  const [category, setCategory] = useState("All");
  const [locationType, setLocationType] = useState("All");
  const [brand, setBrand] = useState("All");
  const [bu, setBu] = useState("All");
  const [section, setSection] = useState("All");
  const [unit, setUnit] = useState("All");


  const resetAllFilters = () => {
  const defaultFilters = {
    ...filters, // current date range same rahegi
     fromDate: "2025-04-01",
    toDate: today,
    unitId: null,
    unitName: "All",
    locationType: null,
    bu: null,
    section: null,
    brand: null,
    category: null
  };

  setFy("All");
  setQuarter("All");
  setCategory("All");
  setLocationType("All");
  setBrand("All");
  setBu("All");
  setSection("All");
  setUnit("All");

 // updateAndHit(defaultFilters);
    setFilters(defaultFilters);
  onFilterChange(defaultFilters);
};

  const updateAndHit = (updated) => {
    setFilters(updated);
    onFilterChange(updated); // 👈 har click pe API hit
  };

  const handleFY = (val) => {
    setFy(val);
    setQuarter("All");

    if (val === "All") {
      updateAndHit({
        ...filters,
        fromDate: "2025-04-01",
        toDate: today
      });
      return;
    }

    const dates = getFYDates(val);

    updateAndHit({
      ...filters,
      fromDate: dates.from,
      toDate: dates.to
    });
  };

  const handleQuarter = (q) => {

  setQuarter(q);

  if (q === "All") {
    return updateAndHit({ ...filters });
  }

  // Current FY if All selected
  let selectedFY = fy;

  if (fy === "All") {

    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1;

    // FY starts from April
    if (currentMonth >= 4) {
      selectedFY = `FY ${currentYear}-${String(currentYear + 1).slice(-2)}`;
    } else {
      selectedFY = `FY ${currentYear - 1}-${String(currentYear).slice(-2)}`;
    }
  }

  const year = selectedFY.replace("FY ", "");
  const startYear = parseInt(year.split("-")[0]);
  const endYear = startYear + 1;

  const map = {
    All: [`$2025-04-01`, today],
    Qtr1: [`${startYear}-04-01`, `${startYear}-06-30`],
    Qtr2: [`${startYear}-07-01`, `${startYear}-09-30`],
    Qtr3: [`${startYear}-10-01`, `${startYear}-12-31`],
    Qtr4: [`${endYear}-01-01`, `${endYear}-03-31`]
  };

  updateAndHit({
    ...filters,
    fromDate: map[q][0],
    toDate: map[q][1]
  });
};
  const genericFilter = (field, val) => {
    updateAndHit({
      ...filters,
      [field]: val === "All" ? null : val
    });
  };

  return (
    <aside className="sidebar left-sidebar">

  <div className="sidebar-label">Filters</div>

  <button
    className="reset-all-btn"
    onClick={resetAllFilters}
  >
    All
  </button>
      
      {/* FY */}
      <FilterSection
        title="Financial Year"
        items={[
          "All",
          ...(filtersData?.fYears?.map(x => `FY ${x.financialYear}`) || [])
        ]}
        selected={fy}
        onSelect={handleFY}
      />
       {/* Quarter */}
      <FilterSection
        title="Quarter"
        items={["All","Qtr1","Qtr2","Qtr3","Qtr4"]}
        selected={quarter}
        onSelect={handleQuarter}
      />
      
      {/* DATE RANGE */}
      <div className="filter-section">

  <div className="filter-header">
    <span className="filter-title">Date Range</span>
  </div>

  <div className="filter-date-inputs">

    <div className="filter-date-field">
      <label className="filter-label">From</label>
      <input
        className="filter-date"
        type="date"
        value={filters.fromDate}
        onChange={(e) => {
          setFy("All");
          setQuarter("All");

          updateAndHit({
            ...filters,
            fromDate: e.target.value
          });
        }}
      />
    </div>

    <div className="filter-date-field">
      <label className="filter-label">To</label>
      <input
        className="filter-date"
        type="date"
        value={filters.toDate}
        onChange={(e) => {
          setFy("All");
          setQuarter("All");

          updateAndHit({
            ...filters,
            toDate: e.target.value
          });
        }}
      />
    </div>

  </div>

      </div>

      {/* Location */}
      <FilterSection
        title="Location Type"
         showSearch={true}
        items={[
          "All",
          ...(filtersData?.locationtype?.map(x => x.unitType) || [])
        ]}
        selected={locationType}
        onSelect={(v)=>{
          setLocationType(v);
          genericFilter("locationType",v);
        }}
      />

      {/* Unit */}
      <FilterSection
        title="Unit"
        showSearch={true}
        items={[
          "All",
          ...(filtersData?.units?.map(x => x.unitName) || [])
        ]}
        selected={unit}
        onSelect={(v)=>{
            setUnit(v);
          const u = filtersData?.units?.find(
  x => x.unitName === v
);

updateAndHit({
  ...filters,
  unitId: u?.unitId || null,
  unitName: v
});
        }}
      />

      {tab === 0 && (
        <>
          
          <FilterSection
            title="BU"
             showSearch={true}
            items={[
              "All",
              ...(filtersData?.buNames?.map(x => x.bu) || [])
            ]}
            selected={bu}
            onSelect={(v)=>{
              setBu(v);
              genericFilter("bu",v);
            }}
          />

          <FilterSection
            title="Brand"
             showSearch={true}
            items={[
              "All",
              ...(filtersData?.brandNames?.map(x => x.brand) || [])
            ]}
            selected={brand}
            onSelect={(v)=>{
              setBrand(v);
              genericFilter("brand",v);
            }}
          />

          <FilterSection
            title="Section"
             showSearch={true}
            items={[
              "All",
              ...(filtersData?.sectionNames?.map(x => x.section) || [])
            ]}
            selected={section}
            onSelect={(v)=>{
              setSection(v);
              genericFilter("section",v);
            }}
          />
        </>
      )}

            {/* Category */}
      <FilterSection
        title="Category"
         showSearch={true}
        items={[
          "All",
          ...(filtersData?.categoryNames?.map(x => x.category) || [])
        ]}
        selected={category}
        onSelect={(v)=>{
          setCategory(v);
          genericFilter("category",v);
        }}
      />

    </aside>
  );
};

/* ---------- Dashboard ---------- */

const TABS = ["Overview", "Production Analysis"];

export const Dashboard3 = () => {

  const dispatch = useDispatch();
  const filtersData = useSelector((state) => state.dashboard.filtersData);

  const [tab, setTab] = useState(0);

    const today = new Date().toISOString().split("T")[0];

const [filters, setFilters] = useState({
  fromDate: "2025-04-01",
  toDate: today,
  unitId: null,
  unitName: "All",
  locationType: null,
  bu: null,
  section: null,
  brand: null,
  category: null
});

  

  // useEffect(() => {
  //   dispatch(fetchDashboardOverviewFilters({}));
  // }, []);

  const handleFilterChange = (payload) => {

    setFilters(payload);
    dispatch(fetchDashboardOverview(payload));
    dispatch(fetchProductionAnalysis(payload)); 
  };

  useEffect(() => {
  const defaultPayload = {
    fromDate: "2025-04-01",
    toDate: new Date().toISOString().split("T")[0],
    unitId: null,
    locationType: null,
    bu: null,
    section: null,
    brand: null,
    category: null
  };

  handleFilterChange(defaultPayload);
    dispatch(fetchDashboardOverviewFilters());
}, []);

  return (
    <div className="dashboard-root">

      <header className="dashboard-header">
        <nav className="dashboard-tabs">
          {TABS.map((item, i) => (
            <button
              key={item}
              className={tab === i ? "dash-tab active" : "dash-tab"}
              onClick={() => setTab(i)}
            >
              {item}
            </button>
          ))}
        </nav>
      </header>

      <div className="dashboard-body">

        <main className="chart-area">
  {tab === 0 ? (
    <Overview
      filters={filters}
      onFilterChange={handleFilterChange}
       filtersData={filtersData}
    />
  ) : (
    <ProductionAnalysis />
  )}
</main>

        <LeftSidebar
  tab={tab}
  onFilterChange={handleFilterChange}
  filtersData={filtersData}
  filters={filters}
  setFilters={setFilters}
/>

      </div>
    </div>
  );
};

export default Dashboard3;