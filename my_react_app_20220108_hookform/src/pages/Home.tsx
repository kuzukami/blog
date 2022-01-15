import React from 'react';
import { useForm, SubmitHandler } from "react-hook-form";
import {List, ListItem} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

type Inputs = {
  example: string,
  exampleRequired: string,
};

const Home = () => {
  const { register, handleSubmit, watch, formState } = useForm<Inputs>();
  const onSubmit: SubmitHandler<Inputs> = (data) => {
    console.log(data);
  }

  console.log(watch("example"));
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              {/* register your input into the hook by invoking the "register" function */}
              <input defaultValue="test" {...register("example", { required: false })} />
            </ListItem><ListItem>
              {/* include validation with required or other standard HTML validation rules */}
              <input defaultValue="test" {...register("exampleRequired", { required: true })} />
              {/* errors will return when field validation fails  */}
              {formState.errors.exampleRequired && <span>This field is required</span>}
            </ListItem>
            <ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home;